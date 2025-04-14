using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryCotroller : MonoBehaviour
{
    //object creation
    private GameObject parent;
    private HandController handController;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        characterController = parent.GetComponent<CharacterController>();
        foreach (Transform child in parent.transform)
        {
            if (child.name == "Hand")
                handController = child.gameObject.GetComponent<HandController>();
        }
        Physics2D.IgnoreLayerCollision(9, 14, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "Gun" && !trigger.gameObject.GetComponent<GunController>().getParented() && handController.getHolding()!=1)//will need to change to item controller once parenting is moved to item
            handController.setChild(trigger.transform);
        if (trigger.gameObject.name == "MedPack")
        {
            characterController.setHealth(characterController.getMaxHealth());
            if(gameObject.name == "TriggerBoundarySpaceMan")
                UIHandler.instance.setHealthValue(characterController.getHealth());
        }
        if (trigger.gameObject.name == "Bullet(Clone)")
        {
            characterController.setBulletStrikeLocation(trigger.gameObject.transform.position);
            characterController.setHealth(characterController.getHealth() - 1f);
            if (gameObject.name == "TriggerBoundarySpaceMan")
                UIHandler.instance.setHealthValue(characterController.getHealth());
        }
    }
}
