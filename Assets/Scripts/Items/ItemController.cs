using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    //object creation
    private HandController handController;
    private Coroutine floatCoroutine;

    //vectors
    private Vector3 originalScale;
    private Vector2 forceBuffer = new Vector2(0, 0);
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
    protected bool floatFlag = false;
    protected int shotBy = 0;

    //public variables
    [SerializeField] private float magnitudeOfFloat = 10f;
    [SerializeField] private float floatSpeed = 10f;
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

        parentLatch = parented;
    }

    public virtual void useItem()
    {
        Debug.Log("Item used");
        //NOTE: each item should have its own override of this
    }

    //this is marked as virtual incase an item needs different flags
    private virtual void parentedFlags()
    {
        GameObject hand = transform.parent.gameObject; //will need to be changed if there are different things to parent to
        handController = hand.GetComponent<HandController>();
        shotBy = hand.layer;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        simulated = false;
        floatFlag = false;
        gravityAffected = false;
        orientToGravity = false;
        grabable = false;
    }

    //this is marked as virtual incase an item needs different flags
    private virtual void notParentedFlags()
    {
        transform.localScale = originalScale;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(forceBuffer, ForceMode2D.Impulse);
        forceBuffer = new Vector2(0, 0);
        handController = null;
        shotBy = 2; //ignore raycast layer
        gravityAffected = true;
        orientToGravity = true;
        updateGravityField = true;
        simulated = true;
        StartCoroutine(grabDelayFunction());
    }

    private IEnumerator grabDelayFunction()
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
        Vector2 offsetPos = transform.position - magnitudeOfFloat * parallel;

        while (true)
        {
            elapsedTime += floatSpeed*Time.deltaTime;

            // Sine oscillation for smooth float motion
            float offset = Mathf.Sin((elapsedTime * frequency)+(Mathf.PI / 2f)) * amplitude;

            // Move along the perpendicular axis relative to starting position
            transform.position = offsetPos + parallel * offset;
            yield return null; // wait until next frame
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            float dot = Vector2.Dot(gravityDirection.normalized, contact.normal);
            if (Mathf.Abs(dot) > .9f)
            {
                floatFlag = true;
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
