using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManTriggerBoundaryController : TriggerBoundaryCotroller
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Bug")
        {
            characterController.hit(trigger.transform);
            UIHandler.instance.setHealthValue(characterController.getHealth());
        }
        base.OnTriggerEnter2D(trigger);
    }

}
