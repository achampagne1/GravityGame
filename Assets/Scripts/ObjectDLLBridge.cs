using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ObjectDLLBridge
{
    [StructLayout(LayoutKind.Sequential)]
    struct GravityPoint
    {
        public float x;
        public float y;
        public float fieldSize;
    }

    public static GameObject findClosestFieldDLL(GameObject self, List<GameObject> gravityPoints){
        GravityPoint[] gravityPointsArr = new GravityPoint[gravityPoints.Count];
        GravityPoint selfStruct = new GravityPoint();
        selfStruct.x = self.transform.position.x;
        selfStruct.y = self.transform.position.y;
        selfStruct.fieldSize = 0f;
        int iterator = 0;
        foreach(GameObject gravityPoint in gravityPoints)
        {
            GravityPoint gravityPointStruct = new GravityPoint();
            gravityPointStruct.x = gravityPoint.transform.position.x;
            gravityPointStruct.y = gravityPoint.transform.position.y;
            gravityPointStruct.fieldSize = gravityPoint.GetComponent<GravityPointController>().getFieldSize();
            gravityPointsArr[iterator]=gravityPointStruct;
            iterator++;
        }
        int closest = bridge(selfStruct, gravityPointsArr,gravityPoints.Count);
        return gravityPoints[closest];
    }

    [DllImport("ObjectMathEngine")]
    private static extern int bridge([In] GravityPoint self, [In] GravityPoint[] gravityPoints, [In] int gravityPointsLen);
}