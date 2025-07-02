using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTriggerBoundaryController : TriggerBoundaryCotroller
{
    private BugController slimeController;
    public override void Start()
    {
        slimeController = transform.parent.GetComponent<BugController>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "TriggerBoundarySpaceMan")
        {
            slimeController.triggerPlayerHit();
        }
        base.OnTriggerEnter2D(trigger);
    }
}
