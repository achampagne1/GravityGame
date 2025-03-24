using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //object creation
    protected Rigidbody2D rb;
    protected Transform planetCenter;

    //public game variables
    public float terminalVelocity = 30f;
    public bool gravityAffected = true;
    public bool orientToGravity = true;

    //game variables
    protected int layerMaskPlanet = 0;
    protected float gravityForceMag = 20f;

    //vectors
    protected Vector2 gravityDirection = new Vector2(0, 0);
    protected Vector2 gravityForce = new Vector2(0, 0);

    protected void calculateStart()
    {
        rb = GetComponent<Rigidbody2D>();
        layerMaskPlanet = LayerMask.GetMask("Default");

        FindClosestField();
        rb.velocity = new Vector2(0, 0); //this can be moifie to have a starting velocity*/
    }

    protected void calculateUpdate()
    {
        calculateGravity();
        if (gravityAffected)
        {
            rb.AddForce(gravityForce);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, terminalVelocity); //terminal velocity
        }

        if(orientToGravity) calculateRotation();
    }

    protected virtual void calculateGravity() 
    {
        FindClosestField();
        //Calculate gravitational force towards the planet
        gravityDirection = (planetCenter.position - transform.position).normalized;
        gravityForce = gravityDirection * gravityForceMag;
    }

    private void FindClosestField() //this might need to be revamped one day. maybe not check for a new gravity field every update
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
}
