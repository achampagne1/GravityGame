using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public struct PermanentItemData
{
    public bool twoHands;
    public Vector2 hand1Pos;
    public Vector2 hand2Pos;
    public float activeArmLength;
    public float passiveArmLength;
    public int sortingOrder;
    public float relaxAngle;
    public IItemEffect itemEffect;
}

public class ItemController : ObjectController
{
    //events
    public event Action itemUsedOnce;
    public event Action itemUsedHold;
    public event Action itemUsedRelease;

    //object creation
    protected HandController handController;
    private Coroutine floatCoroutine;
    protected IItemEffect itemEffect = null;

    //vectors
    protected Vector3 originalScale;
    protected Vector2 forceBuffer = new Vector2(0, 0);

    //private variables
    private float floatCounter = 360f;
    private float heightOffGround = .1f;
    private Coroutine floatItemCoroutine;

    //protected variables
    protected bool parented = false;
    protected bool parentLatch = true;
    protected bool grabable = false;
    protected bool grabableLockout = false;
    protected bool floatFlag = false;
    protected bool floatLockout = false;
    protected int shotBy = 0;

    //serialized fields
    [SerializeField] private float magnitudeOfFloat = .75f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float grabDelay = 1f;
    [SerializeField] private float armLengthActive = .5f;
    [SerializeField] private float armLengthPassive = .5f;
    [SerializeField] private bool twoHands = false;
    [SerializeField] protected Vector2 handOffset1;
    [SerializeField] private Vector2 handOffset2;
    [SerializeField] private float relaxAngle = 0f;

    // Start is called before the first frame update
    public override void Start()
    {
        facingLeft = transform.localScale.x < 0;
        base.Start();
        originalScale = transform.localScale;
        itemEffect = GetComponent<IItemEffect>();

        parented = transform.parent != null;
        if (parented)
            parentedFlags();
        else
            notParentedFlags();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        parented = transform.parent != null;
        if (parented)
        {
            if (!parentLatch)
                parentedFlags();
        }
        else
        {
            if (parentLatch)
                notParentedFlags();
            base.FixedUpdate();
        }

        floatStateMachine();

        rb.AddForce(forceBuffer, ForceMode2D.Impulse); //the force buffer is needed to apply forces after being thrown
        forceBuffer = Vector2.zero;

        parentLatch = parented;
        grabableLockout = false;
    }

    public virtual void useItemOnce()
    {
        itemUsedOnce?.Invoke();
        //Debug.Log("Item used once");
        //NOTE: each item should have its own override of this
    }

    public virtual void useItemHold()
    {
        itemUsedHold?.Invoke();
        //Debug.Log("Item used held");
        //NOTE: each item should have its own override of this
    }

    public virtual void useItemRelease(long holdTime)
    {
        itemUsedRelease?.Invoke();
        //Debug.Log("Item use released");
        //NOTE: each item should have its own override of this
    }

    //this is marked as virtual incase an item needs different flags
    protected virtual void parentedFlags()
    {
        GameObject hand = transform.parent.gameObject; //will need to be changed if there are different things to parent to
        handController = hand.GetComponent<HandController>();
        shotBy = hand.layer;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        simulated = false;
        gravityAffected = false;
        orientToGravity = false;
        grabable = false;
        updateGravityField = false;
        floatFlag = false;

        transform.localPosition = handOffset1;
        transform.localRotation = Quaternion.identity;
        transform.localScale = facingLeft ? new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z) : transform.localScale;
    }

    //this is marked as virtual incase an item needs different flags
    protected virtual void notParentedFlags()
    {
        handController = null;
        shotBy = 2; //ignore raycast layer

        rb.bodyType = RigidbodyType2D.Dynamic;

        simulated = true;
        gravityAffected = true;
        orientToGravity = true;
        if(!grabableLockout)
            StartCoroutine(grabDelayFunction());
        updateGravityField = true;

        transform.localScale = originalScale;
    }

    protected IEnumerator grabDelayFunction()
    {
        yield return new WaitForSeconds(grabDelay);
        grabable = true;
        yield return null;
    }

    private void floatStateMachine()
    {
        if (floatFlag && floatCoroutine == null)
        {
            floatCoroutine = StartCoroutine(floatItem());
        }
        else if (!floatFlag && floatCoroutine!=null)
        {
            StopCoroutine(floatCoroutine);
            floatCoroutine = null;
        }
    }

    private IEnumerator floatItem()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        float amplitude = 0.5f;   
        float frequency = 1f;     
        float elapsedTime = 0f;

        Vector2 parallel = gravityDirection.normalized;
        Vector2 offsetPos = (Vector2)transform.position - magnitudeOfFloat * parallel;

        while (true)
        {
            elapsedTime += floatSpeed*Time.deltaTime;
            float offset = Mathf.Sin((elapsedTime * frequency)+(Mathf.PI / 2f)) * amplitude;
            transform.position = offsetPos + parallel * offset;
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            float dot = Vector2.Dot(gravityDirection.normalized, contact.normal);
            if (!floatLockout)
            {
                if (Mathf.Abs(dot) > .9f)
                {
                    floatFlag = true;
                    return;
                }
            }
            else
            {
                forceBuffer += contact.normal * 10f;
                return;
            }
        }
    }

    public IItemEffect getItemEffect()
    {
        return itemEffect;
    }

    public void setFloatFlag(bool flag)
    {
        floatFlag = flag;
    }

    public void setForceBuffer(Vector2 force)
    {
        forceBuffer = force;
    }

    public void setFacingLeft(bool facingLeft)
    {
        this.facingLeft = facingLeft;
    }

    public bool getFacingLeft()
    {
        return facingLeft;
    }

    public bool getParented()
    {
        return parented;
    }

    public bool getGrabable()
    {
        return grabable;
    }
    
    public PermanentItemData getPermanentItemData()
    {
        PermanentItemData data = new PermanentItemData();
        data.twoHands = twoHands;
        data.hand2Pos = handOffset2;
        data.activeArmLength = armLengthActive;
        data.passiveArmLength = armLengthPassive;
        data.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        data.relaxAngle = relaxAngle;
        data.itemEffect = itemEffect;
        return data;
    }
}
