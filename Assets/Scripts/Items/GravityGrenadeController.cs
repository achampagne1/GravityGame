using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

class GravityGrenadeController : ItemController
{
    //serialized fields
    [SerializeField] float triggerTime = 5f;
    [SerializeField] float lifeTime = 10f;
    [SerializeField] GameObject nakedGravityPointPrefab;
    private GameObject nakedGravityPoint;

    //private variables
    private bool triggeredFlag = false;
    public override void Start()
    {
        base.Start();
    }

    public override void FixedUpdate()
    {
        //you will need to take it out of the simulation when its triggerd.
        if (!triggeredFlag)
        {
            base.FixedUpdate();
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            simulated = false;
            floatFlag = false;
            gravityAffected = false;
            orientToGravity = false;
            grabable = false;
        }
    }

    public override void useItem()
    {
        //this needs to built upon the not parented logic in item controller
        //it needs to override the floating item logic too
        //and grab delay
        StartCoroutine(timeline());
    }

    private IEnumerator timeline()
    {
        transform.SetParent(null);
        grabableLockout = true;
        forceBuffer = new Vector2(10f, 10f);
        yield return new WaitForSeconds(triggerTime);
        nakedGravityPoint = Instantiate(nakedGravityPointPrefab, transform.position, Quaternion.identity);
        nakedGravityPoint.transform.parent = gameObject.transform;
        rb.bodyType = RigidbodyType2D.Static;
        //Destroy(gameObject); //uncomment this when the explosion animation is done

        yield return null;
    }
    
}