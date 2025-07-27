using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryCotroller : MonoBehaviour
{
    //object creation
    protected GameObject parent;
    private HandController handController;
    protected CharacterController characterController;
    private int layerConnectedTo = 2; //2 is ignore raycast

    // Start is called before the first frame update
    public virtual void Start()
    {
        parent = transform.parent.gameObject;
        layerConnectedTo = parent.layer;
        characterController = parent.GetComponent<CharacterController>();
        foreach (Transform child in parent.transform)
        {
            if (child.name == "Hand")
                handController = child.gameObject.GetComponent<HandController>();
        }
        Physics2D.IgnoreLayerCollision(9, 14, true);
    }

    protected virtual void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "Gun" && !trigger.gameObject.GetComponent<GunController>().getParented() && handController.getHolding()!=1)//will need to change to item controller once parenting is moved to item
            handController.setChild(trigger.transform);

        if (trigger.gameObject.name == "MedPack")
        {
            characterController.setHealth(characterController.getMaxHealth());
            if(gameObject.name == "TriggerBoundarySpaceMan")
                UIHandler.instance.setHealthValue(characterController.getHealth());
        }

        if (trigger.gameObject.tag == "Projectile")
        {
            //consult with sean to see if there a better way to get around this
            var bullet = trigger.gameObject.GetComponent<BulletController>();
            var laser = trigger.gameObject.GetComponent<LaserController>();

            if ((bullet != null && bullet.getShotBy() == parent.layer) ||
                (laser != null && laser.getShotBy() == parent.layer))
            {
                return;
            }

            characterController.hit(trigger.transform);
        }
    }

    public int getLayerConnectedTo()
    {
        return layerConnectedTo;
    }
}
