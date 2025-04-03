using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ObjectiveWrapper : ScriptableObject
{
    private Stack<Objective> objectives;
    public ObjectiveWrapper(IEnumerator objectivesEnumerator)
    {
        objectives = new Stack<Objective>(2); //I am only working with 2 objectives at the moment.

        objectivesEnumerator.MoveNext();
        Objective objective2 = new Objective("kill enemy", () =>
        {
            return true; //temporary
        }, (VisualElement)objectivesEnumerator.Current);
        objectives.Push(objective2);

        objectivesEnumerator.MoveNext();
        Objective objective1 = new Objective("get gun", () =>
        {
            GameObject hand = GameObject.Find("SpaceManHand");
            if (hand.transform.childCount == 1)
                return true;
            return false;
        }, (VisualElement)objectivesEnumerator.Current);
        objectives.Push(objective1);
    }
    public Objective getCurrentObjective()
    {
        return objectives.Pop();
    }
}
