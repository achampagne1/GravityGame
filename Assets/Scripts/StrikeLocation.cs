using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

public class StrikeLocation
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vessel
    {
        public float vx;
        public float vy;
        public float xCollider;
        public float yCollider;
        public float radius;
        public float xCollided;
        public float yCollided;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Output
    {
        public float x;
        public float y;
        public float angle;
    }

    public static (Vector2, Quaternion) determineStrikeLocation(GameObject collider, GameObject collided, CircleCollider2D collidedRadius)
    {
        Vessel vessel = new Vessel();
        Rigidbody2D colliderRb = collider.GetComponent<Rigidbody2D>();
        vessel.vx = colliderRb.velocity.x;
        vessel.vy = colliderRb.velocity.y;
        vessel.xCollider = collider.transform.position.x;
        vessel.yCollider = collider.transform.position.y;
        vessel.radius = collidedRadius.radius * Mathf.Max(collided.transform.lossyScale.x, collided.transform.lossyScale.y); //TODO: make it so if its not a circle it doesnt break;
        vessel.xCollided = collided.transform.position.x;
        vessel.yCollided = collided.transform.position.y;
        
        Output output = bridge(vessel);
        Vector2 outputVec = Vector2.zero;
        Quaternion rotation = Quaternion.identity;
        return (outputVec, rotation);
    }

    [DllImport("DetermineStrikeLocation")]
    private static extern Output bridge([In] Vessel vessel);
}
