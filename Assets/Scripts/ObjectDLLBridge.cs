using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ObjectDLLBridge
{

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
        int closest = bridge(selfStruct, gravityPointsArr);
        return gravityPoints[closest];
    }

    [DllImport("objectHelper")]
    private static extern int bridge([In] GravityPoint self, [In] GravityPoint[] gravityPoints);
}