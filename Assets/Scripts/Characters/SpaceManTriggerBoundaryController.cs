using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManTriggerBoundaryController : TriggerBoundaryCotroller
{
    //object creation
    private HandController handController;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        foreach (Transform child in parent.transform)
        {
            if (child.name == "Hand")
                handController = child.gameObject.GetComponent<HandController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D trigger)
    {
        ItemController itemController = trigger.gameObject.GetComponent<ItemController>();
        if (itemController!=null && !itemController.getParented() && !handController.getHolding())
            handController.setChild(trigger.transform);

        /*if (trigger.gameObject.name == "MedPack")
        {
            characterController.setHealth(characterController.getMaxHealth());
            if (gameObject.name == "TriggerBoundarySpaceMan")
                UIHandler.instance.setHealthValue(characterController.getHealth());
        }*/
        base.OnTriggerEnter2D(trigger);
    }

        /*protected override void OnTriggerEnter2D(Collider2D trigger)
        {
            if (trigger.gameObject.tag == "Bug") { 
                characterController.hit(trigger.transform);
                UIHandler.instance.setHealthValue(characterController.getHealth());
            }
            base.OnTriggerEnter2D(trigger);
        }*/

    }
