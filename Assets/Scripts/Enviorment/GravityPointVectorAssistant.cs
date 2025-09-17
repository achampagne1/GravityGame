using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GravityPointVectorAssistant
{

    [DllImport("GravityPointMath", CallingConvention = CallingConvention.Cdecl)]
    public static extern void addGravityPoint(ref GravityPoint gravityPoint);

    [DllImport("GravityPointMath", CallingConvention = CallingConvention.Cdecl)]
    public static extern void removeGravityPoint(ref GravityPoint gravityPoint);

    [DllImport("GravityPointMath", CallingConvention = CallingConvention.Cdecl)]
    public static extern GravityPoint calulateClosestField(ref GravityPoint gravityPoint);

    [DllImport("GravityPointMath", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr returnVec(ref int num);

    public static void printVec()
    {
        int num = 0;
        IntPtr ptr = returnVec(ref num);

        for (int i = 0; i < num; i++)
        {
            IntPtr elementPtr = Marshal.ReadIntPtr(ptr, i * IntPtr.Size);
            GravityPoint gp = Marshal.PtrToStructure<GravityPoint>(elementPtr);

            Debug.Log(gp);
        }
    }
}
