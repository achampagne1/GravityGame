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

    public static (Vector2, Quaternion) determineStrikeLocation(GameObject collider, GameObject collided, float collidedRadius) 
    {
        Vessel vessel = new Vessel();
        Output output = new Output();

        Rigidbody2D colliderRb = collider.GetComponent<Rigidbody2D>();
        vessel.vx = colliderRb.linearVelocity.x;
        vessel.vy = colliderRb.linearVelocity.y;
        vessel.xCollider = collider.transform.position.x;
        vessel.yCollider = collider.transform.position.y;
        vessel.radius = collidedRadius;
        vessel.xCollided = collided.transform.position.x;
        vessel.yCollided = collided.transform.position.y;

        bridge(ref vessel, ref output);
        Vector2 outputVec = new Vector2(output.x,output.y);
        Quaternion rotation = Quaternion.Euler(0, 0, output.angle);
        return (outputVec, rotation);
    }

    [DllImport("DetermineStrikeLocation", CallingConvention = CallingConvention.Cdecl)]
    private static extern void bridge(ref Vessel vessel, ref Output output);
}
