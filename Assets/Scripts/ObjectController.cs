using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //object creation
    protected Rigidbody2D rb;
    protected Transform planetCenter;
    protected Timer groundTimer;
    protected Collider2D collider;

    //public game variables
    public float terminalVelocity = 30f;
    public bool gravityAffected = true;
    public bool orientToGravity = true;

    //game variables
    protected int layerMaskPlanet = 0;
    protected float gravityForceMag = 20f;
    protected bool isGrounded = false;
    protected float heightObject = 0;
    private float groundAngle = 0f;
    private float steepestGrade = 135f;


    //vectors
    protected Vector2 gravityDirection = new Vector2(0, 0);
    protected Vector2 gravityForce = new Vector2(0, 0);
    private Vector2 gravityOverride = new Vector2(0, 0);

    protected void calculateStart()
    {
        rb = GetComponent<Rigidbody2D>();
        layerMaskPlanet = LayerMask.GetMask("Default", "Platforms");
        groundTimer = new Timer(0.4f);

        findClosestField();
        rb.velocity = new Vector2(0, 0); //this can be moifie to have a starting velocity*/

        heightObject = getHeight() + .3f; //the .3 is to allow ground detection even on a slope
    }

    protected void calculateUpdate()
    {
        isGrounded = IsGrounded();
        calculateGravity();
        if (gravityAffected)
        {
            //if (groundAngle < steepestGrade)  //figure out steepest grade later
                rb.AddForce(gravityForce);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, terminalVelocity); //terminal velocity
        }

        if(orientToGravity) calculateRotation();
    }

    protected virtual void calculateGravity() //why is this virtual
    {
        if (gravityOverride != Vector2.zero)
        {
            gravityDirection = gravityOverride.normalized;
            gravityForce = gravityOverride;
        }
        else
        {
            findClosestField();
            //Calculate gravitational force towards the planet
            gravityDirection = (planetCenter.position - transform.position).normalized;
            gravityForce = gravityDirection * gravityForceMag;
        }
    }

    private void findClosestField() //this might need to be revamped one day. maybe not check for a new gravity field every update
    {
        GameObject temp = GameObject.Find("GravityPointsList"); //resuing temp might be a bad idea
        GravityPointsList gravityPointsList = temp.GetComponent<GravityPointsList>();
        List<GameObject> gravityPoints = gravityPointsList.gravityPoints;
        float closestGravityField = 1000f;
        foreach (GameObject gravityPoint in gravityPoints)
        {
            GravityPointController gravityPointController = gravityPoint.GetComponent<GravityPointController>();
            float adjustedDistance = (float)(transform.position - gravityPoint.transform.position).magnitude / gravityPointController.getFieldSize();
            if (adjustedDistance < closestGravityField)
            {
                closestGravityField = adjustedDistance;
                temp = gravityPoint;
            }
        }

        planetCenter = temp.GetComponent<Transform>();
        gravityForceMag = temp.GetComponent<GravityPointController>().getFieldStrength();
    }

    protected virtual void calculateRotation()
    {
            float rotationSmoothTime = 0.05f; // Adjust for more/less smoothness
            float rotationVelocity = 0f;
            float targetAngle = Mathf.Atan2(gravityDirection.y, gravityDirection.x) * Mathf.Rad2Deg + 90f;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle); //this should be done in update if you want to be consistant with all the other update
            //the rotation transform must be modified here directly since the class is virtual. bullet overrides this for its own

    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, gravityDirection, heightObject, layerMaskPlanet);
        bool ground = hit.collider != null;
        if (!ground)
        {
            groundAngle = 0f;
            if(!groundTimer.getIsRunning())
                groundTimer.startTimer();
        }

        else
        {
            groundTimer.resetTimer();
            Vector2 groundNormal = hit.normal;
            groundAngle = Vector2.Angle(gravityDirection, groundNormal);
        }
        return ground;
    }

    private float getHeight()
    {
        Collider2D collider = GetComponent<Collider2D>();

        if (collider is CircleCollider2D circle)
        {
            return collider.bounds.extents.y; // Diameter = 2 * radius
        }
        else if (collider is BoxCollider2D box)
        {
            return box.size.y * Mathf.Abs(box.transform.lossyScale.y); // Adjust for scale
        }
        else if (collider is PolygonCollider2D poly)
        {
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (Vector2 point in poly.points)
            {
                float worldY = poly.transform.TransformPoint(point).y;
                minY = Mathf.Min(minY, worldY);
                maxY = Mathf.Max(maxY, worldY);
            }

            return maxY - minY;
        }

        Debug.LogError("No attached collider");
        return 0f;
    }


    public void setGravityOverride(Vector2 newGravity)
    {
        gravityOverride = newGravity;
    }
}
