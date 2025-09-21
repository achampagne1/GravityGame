using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    //object creation
    private HandController handController;

    //vectors
    private Vector3 originalPosition = Vector3.zero;
    private Vector3 originalScale;
    private Vector2 forceBuffer = new Vector2(0, 0);
    [SerializeField] private Vector2 handOffset;

    //private variables
    private float floatCounter = 360f;
    private Coroutine floatItemCoroutine;
    private bool grabable = false;

    //protected variables
    protected bool parented = false;
    protected bool facingLeft = false;
    protected bool parentLatch = true;
    protected int shotBy = 0;

    //public variables
    public bool floatFlag = false;
    [SerializeField] private float magnitudeOfFloat = .25f;
    [SerializeField] private float flaotSpeed = 100f;
    [SerializeField] private float grabDelay = 1f;

    // Start is called before the first frame update
    public override void Start()
    {
        facingLeft = transform.localScale.x < 0;
        base.Start();
        originalPosition = transform.position;
        originalScale = transform.localScale;

        parented = transform.parent != null;
        if (parented)
        {
            parentedFlags();
        }
        else
        {
            notParentedFlags();
        }
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        parented = transform.parent != null;
        if (parented)
        {
            if (!parentLatch)
            {
                parentedFlags();
            }
            facingLeft = handController.getFacingLeft();
        }
        else
        {
            if (parentLatch)
            {
                notParentedFlags();
            }
            base.FixedUpdate();
        }
        parentLatch = parented;
    }

    public virtual void useItem()
    {
        Debug.Log("Item used");
        //NOTE: each item should have its own override of this
    }

    private void parentedFlags()
    {
        parentingHelper();
        rb.bodyType = RigidbodyType2D.Kinematic;
        floatFlag = false;
        gravityAffected = false;
        orientToGravity = false;
        grabable = false;
    }

    private void notParentedFlags()
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
        //floatFlag=true; 
    }

    private IEnumerator grabDelayFunction()
    {
        yield return new WaitForSeconds(grabDelay);
        grabable = true;
        yield return null;
    }


    private void parentingHelper()
    {
        GameObject hand = transform.parent.gameObject; //will need to be changed if there are different things to parent to
        handController = hand.GetComponent<HandController>();
        shotBy = hand.layer;
    }

    private IEnumerator floatItem()
    {
        orientToGravity = true;
        originalPosition = transform.position;
        while (true)
        {
            floatCounter -= flaotSpeed * Time.deltaTime;

            Vector2 newPosition = new Vector2(
                originalPosition.x + Mathf.Sin(floatCounter) * magnitudeOfFloat * -gravityDirection.x,
                originalPosition.y + Mathf.Sin(floatCounter) * magnitudeOfFloat * -gravityDirection.y
            );

            rb.MovePosition(newPosition);

            if (floatCounter <= 0)
            {
                floatCounter = 360f;
                transform.position = originalPosition;
            }

            yield return null;
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
