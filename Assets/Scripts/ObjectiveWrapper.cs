using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;



public class ObjectiveWrapper : ScriptableObject
{
    private Stack<Objective> objectives;
    public ObjectiveWrapper()
    {
        objectives = new Stack<Objective>(3); //I am only working with 2 objectives at the moment.
    }

    public async Task initializeObjectives()
    {
        string name = "missioncomplete";
        VisualElement visualElement = await createVisualElement(name);
        Objective objective = new Objective(name, () =>
        {
            return false;
        }, visualElement);
        objectives.Push(objective);

        name = "killenemy";
        GameObject enemy = GameObject.Find("SpaceZombie");
        CharacterController zombieController = enemy.GetComponent<CharacterController>();
        visualElement = await createVisualElement(name);
        objective = new Objective(name, () =>
        {
            return zombieController.getHealth() == 0;
        }, visualElement);
        objectives.Push(objective);



        name = "getyourgun";
        GameObject spaceManHand = GameObject.Find("SpaceManHand");
        visualElement = await createVisualElement(name);
        objective = new Objective(name, () =>
        {
            if (spaceManHand.transform.childCount == 1)
            {
                return true;
            }
            return false;
        }, visualElement);
        objectives.Push(objective);
    }


    private Task<VisualElement> createVisualElement(string name)
    {
        var tcs = new TaskCompletionSource<VisualElement>();
        VisualElement visualElement = new VisualElement();

        Addressables.LoadAssetAsync<Texture2D>("Assets/Art/UI/WaypointText/" + name + ".png").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                visualElement.style.backgroundImage = new StyleBackground(handle.Result);
                tcs.SetResult(visualElement);
            }
            else
            {
                Debug.LogError("Failed to load Addressable texture.");
                tcs.SetResult(null);
            }
        };

        return tcs.Task;
    }


    public Objective getNextObjective()
    {
        return objectives.Pop();
    }
}
