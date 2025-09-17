using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

class GravityGrenadeController : ObjectController
{
    //serialized fields
    [SerializeField] float triggerTime = 5f;
    [SerializeField] float lifeTime = 10f;
    [SerializeField] GameObject nakedGravityPointPrefab;
    private GameObject nakedGravityPoint;

    //private variables
    private bool triggeredFlag = false;
    public void Start()
    {
        calculateStart();
        StartCoroutine(timeline());
    }

    public void FixedUpdate()
    {
        //you will need to take it out of the simulation when its triggerd.
        if (!triggeredFlag)
        {
            calculateUpdate();
        }
    }

    private IEnumerator timeline()
    {
        yield return new WaitForSeconds(triggerTime);
        //nakedGravityPoint = Instantiate(nakedGravityPointPrefab, transform.position, Quaternion.identity);
        nakedGravityPoint.transform.parent = gameObject.transform;
        rb.bodyType = RigidbodyType2D.Static;
        
        yield return null;
    }
    
}