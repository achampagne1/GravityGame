using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HandController : CharacterProp
{
    //object creation
    Transform playerBody;  // Assign the player's body transform
    SpacePersonController spacePersonController;
    ItemController itemController;

    //game variables
    [SerializeField] private float armLengthAiming = 4.5f;
    [SerializeField] private float armLengthRelaxed = 3f;
    [SerializeField] private float relaxAngle = 0f;
    [SerializeField] private bool relax = false;
    private Queue<Vector2> delay;
    private float smoothTime = .05f;
    private Vector2 velocity = Vector2.zero;
    private bool facingLeft = false;
    private bool holdingLatch = false;
    private bool facingLeftLatch = false;
    private bool holding = false;
    private GameObject secondHand = null;

    [SerializeField] float holdingRelaxAngle = 0.0f;
    [SerializeField] float inputDirectionTolerance = 0.1f;

    // Start is called before the first frame update
    public override void Start()
    {
        GameObject temp = transform.parent.gameObject; //hand will always have a character parent
        playerBody = temp.GetComponent<Transform>();
        spacePersonController = temp.GetComponent<SpacePersonController>();

        delay = new Queue<Vector2>();
        delay.Enqueue(transform.position);
        holding = transform.childCount == 1;
        if (holding)
            setChild(transform.GetChild(0));

        base.Start();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        facingLeft = spacePersonController.getFacingLeft();

        //NOTE: The second hand is a child of the first hand. Will this cause issues? maybe
        holding = transform.childCount >= 1;

        holdingLatch = holding;
        facingLeftLatch = facingLeft;

        base.FixedUpdate();
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

    public void useHandOnce() //this will need to get expanded to allow for multiple inputs into the item
    {
        if (holding)
            itemController.useItemOnce();
        else
            Debug.Log("Nothing to use");
    }

    public void useHandHold()
    {
        if (holding)
            itemController.useItemHold();
        else
            Debug.Log("Nothing to use");
    }

    public void useHandRelease(long holdTime)
    {
        if (holding)
            itemController.useItemRelease(holdTime);
        else
            Debug.Log("Nothing to use");
    }

    protected override void idleState()
    {
        //TODO: idle state needs to break up into emptyHand and holding something but not aiming, or (future) holding something but not aimable
        if (holding)
            relaxedHolding();
        else
            emptyHand();
    }

    protected override void aimingState()
    {
        originalPosition = new Vector3(armLengthAiming, 0, 0);
        base.aimingState();
    }

    private void relaxedHolding()
    { 
        originalPosition = new Vector3(armLengthRelaxed, 0, 0);
        originalRotation = Quaternion.Euler(0, 0, relaxAngle);
        base.idleState();
    }

    private void emptyHand()
    {
        if (holdingLatch!=holding)
        {
            itemController = null;
            transform.rotation = playerBody.rotation;
            transform.localScale = originalScale;
            if(secondHand != null)
            {
                Destroy(secondHand);
                secondHand = null;
            }
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

    public void setChild(Transform child)
    {
        itemController = child.gameObject.GetComponent<ItemController>();
        //armLength = itemController.getArmLength();
        child.SetParent(gameObject.transform);
        transform.localScale = facingLeft ? new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z) : transform.localScale; //this is for setting the orientation of the hand corrctly
        createSecondHand(child);    
    }

    private void createSecondHand(Transform child)
    {
        secondHand = new GameObject();
        secondHand.name = "secondHand";
        SpriteRenderer sr = secondHand.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        secondHand.transform.parent = transform;
        secondHand.transform.localScale = transform.localScale;
        sr.sortingOrder = child.GetComponent<SpriteRenderer>().sortingOrder - 1;
        secondHand.transform.localPosition = child.GetComponent<ItemController>().getHandOffset2();
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