using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravityFieldController : MonoBehaviour
{
    [SerializeField] protected float fieldStrength = 20f;
    [SerializeField] protected Vector2 direction = new Vector2(0, -1);
    protected Collider2D fieldCollider;
    protected int type = 0; // 0 = directional, 1 = radial

    private void Start()
    {
        fieldCollider = GetComponent<Collider2D>();
        if (fieldCollider is CircleCollider2D)
        {
            type = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(!trigger.isTrigger)
        {
            updateGravityOverride(trigger);
        }
    }

    private void OnTriggerStay2D(Collider2D trigger)
    {
        if (!trigger.isTrigger)
        {
            updateGravityOverride(trigger);
        }
    }

    protected virtual void updateGravityOverride(Collider2D trigger)
    {
        if (type == 1)
        {
            Vector2 directionToCenter = (Vector2)fieldCollider.bounds.center - (Vector2)trigger.transform.position;
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(fieldStrength * directionToCenter.normalized);
        }
        else
        {
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(fieldStrength * (Vector2)(transform.rotation * direction));
        }
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (!trigger.isTrigger)
        {
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(Vector2.zero);
        }
    }
}
