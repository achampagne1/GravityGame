using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

class GravityGrenadeController : ObjectController
{
    //serialized fields
    [SerializedField] float triggerTime = 5f;

    //private variables
    private bool triggeredFlag = false;
    public void Start()
    {
        calculateStart();
        StartCoroutine(trigger());
    }

    public void FixedUpdate()
    {
        calculateUpdate();
    }

    private IEnumerator trigger()
    {
        yield return new WaitForSeconds(triggerTime);
        triggeredFlag = true;
        yield return null;
    }
    
}