using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct Objective 
{
    public string name;
    public Func<bool> completionCondition;
    public VisualElement visualElement;
    public Objective(string name, Func<bool> completionCondition, VisualElement visualElement)
    {
        this.name = name;
        this.completionCondition = completionCondition;
        this.visualElement = visualElement;
    }
}
