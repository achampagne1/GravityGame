using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    //object creation
    Transform playerBody;  // Assign the player's body transform
    SpacePersonController spacePersonController;
    ItemController itemController;

    //game variables
    private Queue<Vector2> delay;
    private float smoothTime = .05f;
    private Vector2 velocity = Vector2.zero;
    private Vector3 inputDirection = Vector3.zero;
    private Vector3 originalScale = Vector3.zero;
    private bool facingLeft = false;
    private bool holdingLatch = false;
    private bool facingLeftLatch = false;
    private bool holding = false;


    // Start is called before the first frame update
    public void Start()
    {
        originalScale = transform.localScale;
        GameObject temp = transform.parent.gameObject; //hand will always have a character parent
        playerBody = temp.GetComponent<Transform>();
        spacePersonController = temp.GetComponent<SpacePersonController>();

        delay = new Queue<Vector2>();
        delay.Enqueue(transform.position);
        holding = transform.childCount == 1;
        if (holding)
            setChild(transform.GetChild(0));

    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        facingLeft = spacePersonController.getFacingLeft();

        //this is for handling if youre holding an item or not
        holding = transform.childCount == 1;
        if (!holding)
            emptyHand();
        else
            holdingSomething();

        holdingLatch = holding;
        facingLeftLatch = facingLeft;
    }

    public void throwItem()
    {
        if (!holding) //theres nothing to throw
            return;

        Transform child = transform.GetChild(0); // Get first child
        child.position = transform.parent.transform.position;
        Vector2 forceLocal = transform.parent.transform.TransformDirection(new Vector2(7f * (facingLeft ? -1 : 1), 7f));
        itemController.setForceBuffer(forceLocal);
        child.SetParent(null); //using transform.SetParent not Item.SetParent
    }

    public void useHand() //this will need to get expanded to allow for multiple inputs into the item
    {
        if (holding)
        {
            itemController.useItem();
        }
    }

    private void emptyHand()
    {
        if (holdingLatch!=holding)
        {
            itemController = null;
            transform.rotation = playerBody.rotation;
            transform.localScale = originalScale;
        }

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
    }

    private void holdingSomething()
    {
        //meed to get offset of item
        float angleRad = Mathf.Atan2(inputDirection.y, inputDirection.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;
        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angleDeg);
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        if (facingLeftLatch != facingLeft)
            transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        transform.position = (Vector2)playerBody.position + offset;
        transform.rotation = rotationQuaternion;
    }

    public void setChild(Transform child)
    {
        itemController = child.gameObject.GetComponent<ItemController>();
        child.SetParent(gameObject.transform);
        child.localRotation = Quaternion.identity;
        Vector2 localPosition = itemController.getHandOffset();
        if (facingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z); //this is for setting the orientation of the hand corrctly
            child.localScale = new Vector3(-child.localScale.x,child.localScale.y,child.localScale.z);
        }
        child.localPosition = (Vector3)localPosition;
    }

    public void setInputDirection(Vector3 inputDirection)
    {
        this.inputDirection = inputDirection;
    }

    public bool getFacingLeft()
    {
        return facingLeft;
    }

    public bool getHolding()
    {
        return holding;
    }

    public GameObject getHoldingObject()
    {
        if (holding)
            return transform.GetChild(0).gameObject;
        else
            return null;
    }

    public void destroyWrapper()
    {
        Destroy(gameObject);
    }


}