using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ObjectDLLBridge
{

    [DllImport("objectHelper")]
    private static extern int Add(int a, int b);
}