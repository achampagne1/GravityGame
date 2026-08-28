using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryCotroller : MonoBehaviour
{
    //object creation
    protected GameObject parent;
    protected CharacterController characterController;
    protected int layerConnectedTo = 2; //2 is ignore raycast

    // Start is called before the first frame update
    public virtual void Start()
    {
        parent = transform.parent.gameObject; //this needs to be reworked
        layerConnectedTo = parent.layer;
        characterController = parent.GetComponent<CharacterController>();
        Physics2D.IgnoreLayerCollision(9, 14, true);
    }

    protected virtual void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Projectile" || trigger.gameObject.tag == "Hazard" || trigger.gameObject.tag == "Melee") 
            characterController.hit(trigger.gameObject);
    }

    public int getLayerConnectedTo()
    {
        return layerConnectedTo;
    }
}
