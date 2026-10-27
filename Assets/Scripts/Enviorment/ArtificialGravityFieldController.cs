using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravityFieldController : MonoBehaviour
{
    [SerializeField] private float fieldStrength = 20f;
    [SerializeField] private Vector2 direction = new Vector2(0, -1);
    private Collider2D fieldCollider;
    private int type = 0; // 0 = directional, 1 = radial

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
            UpdateGravityOverride(trigger);
        }
    }

    private void OnTriggerStay2D(Collider2D trigger)
    {
        if (!trigger.isTrigger)
        {
            UpdateGravityOverride(trigger);
        }
    }

    private void UpdateGravityOverride(Collider2D trigger)
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
