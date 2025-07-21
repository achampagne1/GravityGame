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
    public Func<List<Vector2>> wayPointLocations;
    public Objective(string name, Func<bool> completionCondition, Func<List<Vector2>> wayPointLocations, VisualElement visualElement)
    {
        this.wayPointLocations = wayPointLocations;
        this.name = name;
        this.completionCondition = completionCondition;
        this.visualElement = visualElement;
    }
}
