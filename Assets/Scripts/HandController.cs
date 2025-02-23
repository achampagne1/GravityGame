using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
public class HandController : MonoBehaviour
{
    //object creation
    Transform playerBody;  // Assign the player's body transform
    CharacterController characterController;

    //game variables
    Queue<Vector2> delay;
    float smoothTime = .05f;
    Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.Find("SpaceMan");
        playerBody = temp.GetComponent<Transform>();
        characterController = temp.GetComponent<CharacterController>();
        delay = new Queue<Vector2>();
        delay.Enqueue(transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angleRad = playerBody.rotation.eulerAngles.z * Mathf.Deg2Rad; 
        Vector2 localOffset = new Vector2(characterController.getFacingLeft() ? 0.3f : -0.3f, -0.2f); //calculates the local offset to the body including if the player is facing left or right

        Vector2 offset = new Vector2(
            localOffset.x * Mathf.Cos(angleRad) - localOffset.y * Mathf.Sin(angleRad),
            localOffset.x * Mathf.Sin(angleRad) + localOffset.y * Mathf.Cos(angleRad)
        ); //converts the local offset into a global one

        Vector2 targetPosition = (Vector2)playerBody.position + offset;  //calcluates a target positions

        delay.Enqueue(targetPosition); //adds the target to a queue. this is so the hand follows a path that is sligthly behind the body


        Vector2 delayedTarget = delay.Dequeue(); //gets the old delay


        transform.position = Vector2.SmoothDamp(transform.position, delayedTarget, ref velocity, smoothTime); //smoothly places the hand
    }

}