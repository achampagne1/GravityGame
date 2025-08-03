using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GravityPointController : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    struct GravityPoint
    {
        public float x;
        public float y;
        public float fieldSize;
    }

    GravityPoint gravityPoint = new GravityPoint();

    [SerializeField] private float fieldStrength = 20f;
    [SerializeField] private float fieldSize = 100.0f; //divide by this number


    public float getFieldStrength()
    {
        return fieldStrength;
    }

    public float getFieldSize()
    {
        return fieldSize;
    }

    // Start is called before the first frame update
    void Awake()
    {
        gravityPoint.x = transform.position.x;
        gravityPoint.y = transform.position.y;
        gravityPoint.fieldSize = fieldSize;

        addGravityPoint(ref gravityPoint);
    }

    [DllImport("GravityPointMath")]
    private static extern void addGravityPoint(ref GravityPoint gravityPoint);
}
