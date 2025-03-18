using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    //object creation
    Transform playerBody;  // Assign the player's body transform
    CharacterController characterController;

    //game variables
    private Queue<Vector2> delay;
    private float smoothTime = .05f;
    private Vector2 velocity = Vector2.zero;
    private bool facingLeft = false;
    private int holding = 0;

    // Start is called before the first frame update
    public void Start()
    {
        GameObject temp = transform.parent.gameObject; //hand will always have a character parent
        playerBody = temp.GetComponent<Transform>();
        characterController = temp.GetComponent<CharacterController>();

        delay = new Queue<Vector2>();
        delay.Enqueue(transform.position);
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (characterController.getThrow()&& transform.childCount!=0)
        {
            Transform child = transform.GetChild(0); // Get first child
            GunController childController = child.gameObject.GetComponent<GunController>();
            childController.setForceBuffer(new Vector2(5f, 3f));
            child.SetParent(null);
        }

        holding = transform.childCount;
        if (holding == 0)
            emptyHand();
        else if (holding == 1)
            holdingSomething();
        else
            Debug.LogError("too many children in hand");
    }

    private void emptyHand()
    {
        Vector2 localOffset = new Vector2(characterController.getFacingLeft() ? .5f : -.5f, -.1f); //calculates the local offset to the body including if the player is facing left or right
        float angleRad = playerBody.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(
            localOffset.x * Mathf.Cos(angleRad) - localOffset.y * Mathf.Sin(angleRad),
            localOffset.x * Mathf.Sin(angleRad) + localOffset.y * Mathf.Cos(angleRad)
        ); //converts the local offset into a global one
        Vector2 targetPosition = (Vector2)playerBody.position + offset;  //calcluates a target positions
        delay.Enqueue(targetPosition); //adds the target to a queue. this is so the hand follows a path that is sligthly behind the body
        Vector2 delayedTarget = delay.Dequeue(); //gets the old delay
        transform.position = Vector2.SmoothDamp(transform.position, delayedTarget, ref velocity, smoothTime); //smoothly places the hand
    }

    private void holdingSomething()
    {
        Vector2 rotation = getMouseDirection(Input.mousePosition, playerBody.rotation);
        float angleRad = Mathf.Atan2(rotation.y, rotation.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;
        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angleDeg);
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        if (characterController.getFacingLeft() && facingLeft == false)
        {
            transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            facingLeft = true;
        }
        else if (!characterController.getFacingLeft() && facingLeft == true)
        {
            transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            facingLeft = false;
        }
        transform.position = (Vector2)playerBody.position + offset;
        transform.rotation = rotationQuaternion;
    }

    private static Vector3 getMouseDirection(Vector3 mousePosition, Quaternion playerRotation)
    {
        //This function was written by chat GPT
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(mousePosition.x, mousePosition.y) - screenCenter;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 direction3D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0f);
        Vector3 rotatedDirection = playerRotation * direction3D;
        return new Vector2(rotatedDirection.x, rotatedDirection.y).normalized;
    }

    public bool getClick()
    {
        return characterController.getClick();
    }

    public bool getFacingLeft()
    {
        return facingLeft;
    }


}