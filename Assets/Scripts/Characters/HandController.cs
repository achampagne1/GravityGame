using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HandController : CharacterProp
{
    //object creation
    ItemController itemController;

    //game variables
    [SerializeField] private bool relax = false;
    [SerializeField] private AudioClip relaxClip; //temporarirly in hand controller
    private float armLengthActive = 0;
    private float armLengthPassive = 0;
    private bool twoHandItem = false;
    private Vector2 hand2Offset = new Vector2(0, 0);
    private int itemSortingOrder = 0;
    private Queue<Vector2> delay;
    private float smoothTime = .05f;
    private Vector2 velocity = Vector2.zero;
    private bool holdingLatch = false;
    private bool facingLeftLatch = false;
    private bool holding = false;
    private GameObject secondHand = null;
    private float timeLastUsed = 0.0f;
    private float relaxAngle = 0.0f;

    // Start is called before the first frame update
    public override void Start()
    {
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
        if (stateLatch == CHARACTERSTATE.AIMING && characterState == CHARACTERSTATE.IDLE)
        {
            SoundManager.instance.playSound(relaxClip, transform, 1f);
        }

        holding = GetComponentInChildren<ItemController>() != null;

        base.FixedUpdate();
        itemController.setFacingLeft(facingLeft);

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

    public void useHandOnce() //this will need to get expanded to allow for multiple inputs into the item
    {
        timeLastUsed = Time.realtimeSinceStartup;
        if (holding)
            itemController.useItemOnce();
        else
            Debug.Log("Nothing to use");
    }

    public void useHandHold()
    {
        timeLastUsed = Time.realtimeSinceStartup;
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

    public float getTimeLastUsedDiff()
    {
        if(timeLastUsed == 0.0f)
        {
            timeLastUsed = Time.realtimeSinceStartup;
            return timeLastUsed;
        }
        else
        {
            float tempTime = Time.realtimeSinceStartup - timeLastUsed;
            return tempTime;
        }
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
        originalPosition = new Vector3(armLengthActive, 0, 0);
        base.aimingState();
    }

    private void relaxedHolding()
    { 
        originalRotation = Quaternion.Euler(0, 0, relaxAngle);
        originalPosition = originalRotation * new Vector3(armLengthPassive, 0, 0);
        base.idleState();
    }

    private void emptyHand()
    {
        if (holdingLatch!=holding)
        {
            itemController.itemUsedOnce -= useParentedEffectRequested;
            itemController = null;
            transform.rotation = transform.parent.rotation;
            transform.localScale = originalScale;
            if(secondHand != null)
            {
                Destroy(secondHand);
                secondHand = null;
            }
        }

        Vector2 localOffset = new Vector2(facingLeft ? .5f : -.5f, -.1f); //calculates the local offset to the body including if the player is facing left or right
        float angleRad = transform.parent.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(
            localOffset.x * Mathf.Cos(angleRad) - localOffset.y * Mathf.Sin(angleRad),
            localOffset.x * Mathf.Sin(angleRad) + localOffset.y * Mathf.Cos(angleRad)
        ); //converts the local offset into a global one
        Vector2 targetPosition = (Vector2)transform.parent.position + offset;  //calcluates a target positions
        delay.Enqueue(targetPosition); //adds the target to a queue. this is so the hand follows a path that is sligthly behind the body
        Vector2 delayedTarget = delay.Dequeue(); //gets the old delay
        transform.position = Vector2.SmoothDamp(transform.position, delayedTarget, ref velocity, smoothTime); //smoothly places the hand
    }

    public void setChild(Transform child)
    {
        itemController = child.gameObject.GetComponent<ItemController>();
        setItemData(itemController);
        child.SetParent(gameObject.transform);
        transform.localScale = facingLeft ? new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z) : transform.localScale; //this is for setting the orientation of the hand corrctly    
        itemController.itemUsedOnce += useParentedEffectRequested;

        if(twoHandItem)
            createSecondHand();    
    }

    public void setItemData(ItemController itemController)
    {
        PermanentItemData newItem = itemController.getPermanentItemData();
        armLengthActive           = newItem.activeArmLength;
        armLengthPassive          = newItem.passiveArmLength;
        twoHandItem               = newItem.twoHands;
        hand2Offset               = newItem.hand2Pos;
        itemSortingOrder          = newItem.sortingOrder;
        relaxAngle                = newItem.relaxAngle;
        itemParentedEffect        = newItem.parentedEffect;
    }

    public void resetItemData()
    {
        armLengthActive           = 0.0f;
        armLengthPassive          = 0.0f;
        twoHandItem               = false;
        hand2Offset               = Vector2.zero;
        itemSortingOrder          = 0;
        relaxAngle                = 0.0f;
        itemParentedEffect        = null;
    }

    private void createSecondHand()
    {
        secondHand = new GameObject();
        secondHand.name = "secondHand";
        SpriteRenderer sr = secondHand.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        secondHand.transform.parent = transform;
        secondHand.transform.localScale = transform.localScale;
        sr.sortingOrder = itemSortingOrder - 1;
        secondHand.transform.localPosition = hand2Offset;
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