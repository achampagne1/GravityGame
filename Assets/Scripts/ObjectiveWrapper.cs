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
            return false; //temporary
        }, (VisualElement)objectivesEnumerator.Current);
        objectives.Push(objective2);

        objectivesEnumerator.MoveNext();
        Objective objective1 = new Objective("get gun", () =>
        {
            GameObject hand = GameObject.Find("SpaceManHand"); //figure out a way to not use find every time
            if (hand.transform.childCount == 1)
                return true;
            return false;
        }, (VisualElement)objectivesEnumerator.Current);
        objectives.Push(objective1);
    }

    private VisualElement createVisualElement(string name)
    {
        VisualElement visualElement = new VisualElement();
        Texture2D texture = Resources.Load<Texture2D>("name");
        if (texture != null)
        {
            // Set the background image
            imageElement.style.backgroundImage = new StyleBackground(texture);
            return visualElement;
        }
        else
        {
            Debug.LogError("Failed to load texture! Make sure it's in a Resources folder.");
            return null;
        }
    }
    public Objective getNextObjective()
    {
        return objectives.Pop();
    }
}
