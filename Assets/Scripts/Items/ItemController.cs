using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    //object creation
    private Transform playerBody;
    private HandController handController;

    //vectors
    private Vector3 originalPosition = Vector3.zero;

    //private variables
    private float floatCounter = 360f;
    private Coroutine floatItemCoroutine;

    //protected variables
    protected bool parented = false;
    private bool facingLeft = false;
    private bool parentLatch = true;
    protected int shotBy = 0;

    //public variables
    public bool floatFlag = false;
    [SerializeField] float magnitudeOfFloat = .25f;
    [SerializeField] float flaotSpeed = 100f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        originalPosition = transform.position;

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

        if(throwTimer.checkTimer())
            Physics2D.IgnoreLayerCollision(13, 14, false);
    }

    private void parentedFlags()
    {
        parentingHelper();
        rb.bodyType = RigidbodyType2D.Kinematic;
        floatFlag = false;
        gravityAffected = false;
        orientToGravity = false;
    }

    private void notParentedFlags()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(forceBuffer, ForceMode2D.Impulse);
        forceBuffer = new Vector2(0, 0);
        handController = null;
        playerBody = null;
        shotBy = 2; //ignore raycast layer
        gravityAffected = false;
        orientToGravity = true;
        //floatFlag=true; 
    }

    public void setParent(GameObject parent)
    {
        transform.SetParent(parent.transform); //slightly different method
        transform.localRotation = Quaternion.identity;
        if (parent.gameObject.GetComponent<HandController>().getFacingLeft()!=facingLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            transform.localPosition = new Vector3(-3f, -1f, 0f); //for setting location of gun in hand
            facingLeft = !facingLeft;
        }
        else
            transform.localPosition = new Vector3(3f, 1f, 0f);
    }

    private void parentingHelper()
    {
        GameObject temp = transform.parent.gameObject.transform.parent.gameObject; //this is the gameObject of the character
        GameObject hand = transform.parent.gameObject; //will need to be changed if there are different things to parent to
        handController = hand.GetComponent<HandController>();
        shotBy = hand.layer;
        playerBody = temp.GetComponent<Transform>(); //I want to get rid of the need for the player body and jsut ude the hand but idk how
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
    
    public bool getFacingLeft()
    {
        return facingLeft;
    }

    public bool getParented()
    {
        return parented;
    }

}
