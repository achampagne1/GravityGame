using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceWomanController : MonoBehaviour
{
    //object creation
    Rigidbody2D rb;
    CircleCollider2D circleColliderPlayer;
    Transform planetCenter;

    //constants
    public float gravitationalConstant = 10f;
    public float planetMass = 200f;
    public float gravityForceMag = 20f;

    //game variables
    float distanceSquared = 0;

    //vectors
    Vector2 gravityDirection = new Vector2(0, 0);
    Vector2 gravityForce = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject temp = GameObject.Find("Planet");
        planetCenter = temp.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Calculate gravitational force towards the planet
        gravityDirection = (planetCenter.position - transform.position).normalized;
        distanceSquared = (planetCenter.position - transform.position).sqrMagnitude;
        gravityForce = gravityDirection * gravityForceMag;

        // Create a quaternion representing the desired rotation angle around the y-axis
        float angle = Mathf.Atan2(gravityDirection.y, gravityDirection.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.Euler(0f, 0f, 90f + angle);
        transform.rotation = desiredRotation;

        rb.velocity = gravityForce;
    }
}
