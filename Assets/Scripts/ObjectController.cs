using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //object creation
    protected Rigidbody2D rb;
    protected Transform planetCenter;

    //constants
    protected float gravityForceMag = 20f;

    //game variables
    protected int layerMaskPlanet = 0;

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

        calculateRotation();

        rb.AddForce(gravityForce);
    }

    protected virtual void calculateGravity() 
    {
        FindClosestField();
        //Calculate gravitational force towards the planet
        gravityDirection = (planetCenter.position - transform.position).normalized;
        gravityForce = gravityDirection * gravityForceMag;
    }

    private void FindClosestField()
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
    }

    protected virtual void calculateRotation()
    {
        // Create a quaternion representing the desired rotation angle around the y-axis
        float angle = Mathf.Atan2(gravityDirection.y, gravityDirection.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.Euler(0f, 0f, 90f + angle);
        transform.rotation = desiredRotation;
    }


}
