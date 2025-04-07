using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    //object creation
    Transform playerBody;  // Assign the player's body transform
    CharacterController characterController;
    GunController gunController;

    //game variables
    private Queue<Vector2> delay;
    private float smoothTime = .05f;
    private Vector2 velocity = Vector2.zero;
    private Vector3 inputDirection = Vector3.zero;
    private bool facingLeft = false;
    private bool holdingLatch = false;
    private int holding = 0;

    // Start is called before the first frame update
    public void Start()
    {
        GameObject temp = transform.parent.gameObject; //hand will always have a character parent
        playerBody = temp.GetComponent<Transform>();
        characterController = temp.GetComponent<CharacterController>();

        delay = new Queue<Vector2>();
        delay.Enqueue(transform.position);
        holding = transform.childCount;
        if (holding != 0)
            setChild(transform.GetChild(0));

        //NOTE: currently holding only fully works for guns
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        //this is just for the throwing action
        if (characterController.getThrow()&& transform.childCount!=0)
        {
            Transform child = transform.GetChild(0); // Get first child
            GunController childController = child.gameObject.GetComponent<GunController>(); //will be chagned to item controller
            childController.setForceBuffer(new Vector2(15f, 6f));
            child.SetParent(null); //using transform.SetParent not Item.SetParent
        }

        //this is for handling if youre holding an item or not
        holding = transform.childCount;
        if (holding == 0)
            emptyHand();
        else if (holding == 1)
            holdingSomething();
        else
            Debug.LogError("too many children in hand");

    }

    public void useHand()
    {
        if (holding == 1)
            gunController.shootWrapper(); //currently jsut guns
    }

    private void emptyHand()
    {
        if(holdingLatch)
            gunController = null;

        facingLeft = characterController.getFacingLeft();
        Vector2 localOffset = new Vector2(facingLeft ? .5f : -.5f, -.1f); //calculates the local offset to the body including if the player is facing left or right
        float angleRad = playerBody.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(
            localOffset.x * Mathf.Cos(angleRad) - localOffset.y * Mathf.Sin(angleRad),
            localOffset.x * Mathf.Sin(angleRad) + localOffset.y * Mathf.Cos(angleRad)
        ); //converts the local offset into a global one
        Vector2 targetPosition = (Vector2)playerBody.position + offset;  //calcluates a target positions
        delay.Enqueue(targetPosition); //adds the target to a queue. this is so the hand follows a path that is sligthly behind the body
        Vector2 delayedTarget = delay.Dequeue(); //gets the old delay
        transform.position = Vector2.SmoothDamp(transform.position, delayedTarget, ref velocity, smoothTime); //smoothly places the hand
        holdingLatch = false;
    }

    private void holdingSomething()
    {
        if (!holdingLatch)
            setChild(transform.GetChild(0));

        Vector2 rotation = getMouseDirection(inputDirection, playerBody.rotation);
        float angleRad = Mathf.Atan2(rotation.y, rotation.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;
        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angleDeg);
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        if (characterController.getFacingLeft() != facingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            facingLeft = !facingLeft;
        }
        transform.position = (Vector2)playerBody.position + offset;
        transform.rotation = rotationQuaternion;
        holdingLatch = true;
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

    public void setChild(Transform child)
    {
        gunController = child.gameObject.GetComponent<GunController>();
        gunController.setParent(gameObject);
    }

    public void setInputDirection(Vector3 inputDirection)
    {
        this.inputDirection = inputDirection;
    }

    public bool getFacingLeft()
    {
        return facingLeft;
    }


}