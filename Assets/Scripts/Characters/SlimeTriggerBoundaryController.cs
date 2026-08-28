using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTriggerBoundaryController : TriggerBoundaryCotroller, IDamager
{
    //game variables
    [SerializeField] float damageVariable = 1f;
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
        else if(trigger.gameObject.transform.tag== "Melee")
        {
            return;
        }
        base.OnTriggerEnter2D(trigger);
    }

    public bool damage(GameObject hitGameObject)
    {
        IHealth health = hitGameObject.GetComponent<IHealth>();
        health.setHealth(health.getHealth() - damageVariable);
        return true;
    }

    public float getDamage()
    {
        return damageVariable;
    }
}
