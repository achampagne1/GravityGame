using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryController : MonoBehaviour
{
    //object creation
    protected ObjectController parentController;
    protected int layerConnectedTo = 2; //2 is ignore raycast

    // Start is called before the first frame update
    public virtual void Start()
    {
        layerConnectedTo = transform.parent.gameObject.layer;
        parentController = transform.parent.gameObject.GetComponent<ObjectController>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D trigger)
    {
        parentController.triggerLogic(trigger);
    }

    public int getLayerConnectedTo()
    {
        return layerConnectedTo;
    }
}
