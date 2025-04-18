using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    //object creation
    Transform playerBody;  // Assign the player's body transform
    SpacePersonController spacePersonController;
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
        spacePersonController = temp.GetComponent<SpacePersonController>();

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

        //this is for handling if youre holding an item or not
        holding = transform.childCount;
        if (holding == 0)
            emptyHand();
        else if (holding == 1)
            holdingSomething();
        else
            Debug.LogError("too many children in hand");

    }

    public void throwItem()
    {
        if (transform.childCount == 0) //theres nothing to throw
            return;
        Transform child = transform.GetChild(0); // Get first child
        child.position = transform.parent.transform.position;
        GunController childController = child.gameObject.GetComponent<GunController>(); //will be chagned to item controller
        Vector2 forceLocal = transform.parent.transform.TransformDirection(new Vector2(10f * (facingLeft ? -1 : 1), 18f));
        childController.setForceBuffer(forceLocal);
        child.SetParent(null); //using transform.SetParent not Item.SetParent
    }

    public void useHand()
    {
        if (holding == 1)
        {
            gunController.setShootDirection(inputDirection);
            gunController.shootWrapper(); //currently jsut guns
        }
    }

    private void emptyHand()
    {
        if(holdingLatch)
            gunController = null;

        facingLeft = spacePersonController.getFacingLeft();
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

        float angleRad = Mathf.Atan2(inputDirection.y, inputDirection.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;
        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angleDeg);
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        if (spacePersonController.getFacingLeft() != facingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            facingLeft = !facingLeft;
        }
        transform.position = (Vector2)playerBody.position + offset;
        transform.rotation = rotationQuaternion;
        holdingLatch = true;
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

    public int getHolding()
    {
        return holding;
    }

    public void destroyWrapper()
    {
        Destroy(gameObject);
    }


}