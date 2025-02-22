using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandController : MonoBehaviour
{
    Transform playerBody;  // Assign the player's body transform
    CharacterController characterController; 
    float smoothTime = .00001f;

    Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.Find("SpaceMan");
        playerBody = temp.GetComponent<Transform>();
        characterController = temp.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angleRad = playerBody.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 localOffset = new Vector2(characterController.getFacingLeft() ? 0.3f : -.3f, -0.2f);

        Vector2 offset = new Vector2(
        localOffset.x * Mathf.Cos(angleRad) - localOffset.y * Mathf.Sin(angleRad),
        localOffset.x * Mathf.Sin(angleRad) + localOffset.y * Mathf.Cos(angleRad)
        );
        Vector2 targetPosition = playerBody.position;
        targetPosition += offset;
        transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.rotation = playerBody.rotation;
    }
}