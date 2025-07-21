using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/**
 * IN CASE YOU EVER FORGET
 * objectives contain some information
 * A name, 
 * the condition that needs to be completed. this is a lambda function that is run in the level manager. this is coupled with Wait Until
 * the visual element. this is just the actual .png that is loaded and displayed. 
 * the wayPointLocations. this is a lambda function that returns the locations that the way point points to
 * the reason its a lambda function and not jsut a list of vectors is the locations of what the waypoint is pointing to can change dynamically, or be destroyed all together
 **/
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
