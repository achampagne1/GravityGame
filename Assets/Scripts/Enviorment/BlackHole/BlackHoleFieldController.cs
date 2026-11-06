using UnityEngine;

public class BlackHoleFieldController : ArtificialGravityFieldController
{
    protected override void updateGravityOverride(Collider2D trigger)
    {
        Vector2 directionToCenter = (Vector2)fieldCollider.bounds.center - (Vector2)trigger.transform.position;
        float distanceToCenter = directionToCenter.magnitude;

        if (distanceToCenter < 0.0001f)
        {
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(Vector2.zero);
            return;
        }

        float gravityStrength = fieldStrength / Mathf.Max(distanceToCenter * distanceToCenter, 0.0001f);
        trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(gravityStrength * directionToCenter.normalized);
    }
}
