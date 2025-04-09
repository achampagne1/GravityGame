using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryCotroller : MonoBehaviour
{
    //object creation
    private HandController characterHand;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        characterHand = GameObject.Find("Hand").GetComponent<HandController>();
        character = GetComponentInParent<CharacterController>();
        Physics2D.IgnoreLayerCollision(9, 14, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "Gun" && !trigger.gameObject.GetComponent<GunController>().getParented())//will need to change to item controller once parenting is moved to item
            characterHand.setChild(trigger.transform);
        if (trigger.gameObject.name == "MedPack")
        {
            character.setHealth(character.getMaxHealth());
            if(gameObject.name == "TriggerBoundarySpaceMan")
                UIHandler.instance.setHealthValue(character.getHealth());
        }
        if (trigger.gameObject.name == "Bullet(Clone)")
        {
            character.setHealth(character.getHealth() - 1f);
            if (gameObject.name == "TriggerBoundarySpaceMan")
                UIHandler.instance.setHealthValue(character.getHealth());
        }
    }
}
