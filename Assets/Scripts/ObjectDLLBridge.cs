using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ObjectDLLBridge
{
    public static bool dataMarshalFlag = false;
    [StructLayout(LayoutKind.Sequential)]
    struct GravityPoint
    {
        public float x;
        public float y;
        public float fieldSize;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitOnLoad()
    {
        dataMarshalFlag = false;
    }

    public static void marshalData(List<GameObject> gravityPoints)
    {
        if (dataMarshalFlag)
            return;

        GravityPoint[] gravityPointsArr = new GravityPoint[gravityPoints.Count];

        int iterator = 0;
        foreach (GameObject gravityPoint in gravityPoints)
        {
            GravityPoint gravityPointStruct = new GravityPoint();
            gravityPointStruct.x = gravityPoint.transform.position.x;
            gravityPointStruct.y = gravityPoint.transform.position.y;
            gravityPointStruct.fieldSize = gravityPoint.GetComponent<GravityPointController>().getFieldSize();
            gravityPointsArr[iterator] = gravityPointStruct;
            iterator++;
        }
        //I know its misspelled
        recieveData(gravityPointsArr, gravityPointsArr.Length);
        dataMarshalFlag = true;
    }

    public static int findClosestFieldDLL(GameObject self){
        GravityPoint selfStruct = new GravityPoint();
        selfStruct.x = self.transform.position.x;
        selfStruct.y = self.transform.position.y;
        selfStruct.fieldSize = 0f;
        return bridge(selfStruct);
    }

    [DllImport("ObjectMathEngine")]
    private static extern int bridge([In] GravityPoint self);

    [DllImport("ObjectMathEngine")]
    private static extern void recieveData([In] GravityPoint[] gravityPoints, [In] int gravityPointsLen);

}