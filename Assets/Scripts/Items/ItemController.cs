using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    //object creation
    protected HandController handController;
    private Coroutine floatCoroutine;

    //vectors
    protected Vector3 originalScale;
    protected Vector2 forceBuffer = new Vector2(0, 0);
    [SerializeField] private Vector2 handOffset;

    //private variables
    private float floatCounter = 360f;
    private float heightOffGround = .1f;
    private Coroutine floatItemCoroutine;

    //protected variables
    protected bool parented = false;
    protected bool facingLeft = false;
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

    // Start is called before the first frame update
    public override void Start()
    {
        facingLeft = transform.localScale.x < 0;
        base.Start();
        originalScale = transform.localScale;

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
            facingLeft = handController.getFacingLeft();
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
        //Debug.Log("Item used once");
        //NOTE: each item should have its own override of this
    }

    public virtual void useItemHold()
    {
        //Debug.Log("Item used held");
        //NOTE: each item should have its own override of this
    }

    public virtual void useItemRelease(long holdTime)
    {
        //Debug.Log("Item use released");
        //NOTE: each item should have its own override of this
    }

    //this is marked as virtual incase an item needs different flags
    protected virtual void parentedFlags()
    {
        GameObject hand = transform.parent.gameObject; //will need to be changed if there are different things to parent to
        handController = hand.GetComponent<HandController>();
        shotBy = hand.layer;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        simulated = false;
        gravityAffected = false;
        orientToGravity = false;
        grabable = false;
        updateGravityField = false;
        floatFlag = false;

        transform.localPosition = handOffset;
        transform.localRotation = Quaternion.identity;
        transform.localScale = handController.getFacingLeft() ? new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z) : transform.localScale;
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
        rb.velocity = Vector2.zero;
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

    public void setFloatFlag(bool flag)
    {
        floatFlag = flag;
    }

    public void setForceBuffer(Vector2 force)
    {
        forceBuffer = force;
    }

    public Vector2 getHandOffset()
    {
        return handOffset;
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

}
