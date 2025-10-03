using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GravityPointController : MonoBehaviour
{

    private GravityPoint gravityPoint = new GravityPoint();

    [SerializeField] private float fieldStrength = 15f;
    [SerializeField] private float fieldSize = 200.0f; //divide by this number


    public float getFieldStrength()
    {
        return fieldStrength;
    }

    public float getFieldSize()
    {
        return fieldSize;
    }

    // Start is called before the first frame update
    public void Awake()
    {
        createPoint();
    }

    public void createPoint()
    {
        gravityPoint.x = transform.position.x;
        gravityPoint.y = transform.position.y;
        gravityPoint.fieldSize = fieldSize;
        gravityPoint.fieldStrength = fieldStrength;
        GravityPointVectorAssistant.addGravityPoint(ref gravityPoint);
    }

    void OnDestroy()
    {
        GravityPointVectorAssistant.removeGravityPoint(ref gravityPoint);
    }
}
