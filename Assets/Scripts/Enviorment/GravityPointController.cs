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
    public virtual void Awake()
    {
        createPoint();
    }

    public void createPoint()
    {
        gravityPoint.x = transform.position.x;
        gravityPoint.y = transform.position.y;
        gravityPoint.fieldSize = fieldSize;
        addGravityPoint(ref gravityPoint);
    }

    void OnDestroy()
    {
        removeGravityPoint(ref gravityPoint);
    }

    [DllImport("GravityPointMath", CallingConvention = CallingConvention.Cdecl)]
    private static extern void addGravityPoint(ref GravityPoint gravityPoint);

    [DllImport("GravityPointMath", CallingConvention = CallingConvention.Cdecl)]
    private static extern void removeGravityPoint(ref GravityPoint gravityPoint);
}
