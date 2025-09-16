using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

class GravityGrenadeController : ObjectController
{
    //serialized fields
    [SerializedField] float triggerTime = 5f;
    [SerializedField] float lifeTime = 10f;
    [SerializedField] GameObject nakedGravitySourcePrefab;
    private GameObject nakedGravitySource;
    private GravitySourceController gravitySourceController;

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
        nakedGravitySource = Instantiate(nakedGravitySourcePrefab, transform.position, Quaternion.identity);
        nakedGravitySource.transform.parent - gameObject;
        triggeredFlag = true;
        Destroy(gameObject, lifeTime);
        yield return null;
    }
    
}