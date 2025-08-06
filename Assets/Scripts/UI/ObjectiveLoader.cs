using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;


//this should only be used for setting up the objectives
public class ObjectiveLoader : ScriptableObject
{
    private Dictionary<string, Objective> objectives = new Dictionary<string, Objective>();
    public ObjectiveLoader()
    {

    }

    public async Task initializeObjectives()
    {
        string name = "defeatthegreenteam";
        GameObject enemies = GameObject.Find("Enemies");
        VisualElement visualElement = await createVisualElement(name);
        objectives[name] = new Objective(name, () =>
        {
            foreach (Transform enemy in enemies.transform)
            {
                if (enemy.gameObject.tag == "SpaceZombie")
                {
                    return false;
                }
            }
            return true;
        }, () =>
        {
            List<Vector2> outputs = new List<Vector2>();
            foreach (Transform enemy in enemies.transform)
            {
                if (enemy.gameObject.tag == "SpaceZombie")
                {
                    outputs.Add((Vector2)enemy.transform.position);
                }
            }
            return outputs;
        }, visualElement);


        name = "gettoteleporter";
        GameObject enemySpawnTrigger = GameObject.Find("EnemySpawnTrigger");
        PlanetTrigger enemySpawnTriggerScript = enemySpawnTrigger.GetComponent<PlanetTrigger>();
        visualElement = await createVisualElement(name);
        objectives[name] = new Objective(name, () =>
        {
            return enemySpawnTriggerScript.checkIfOverlapping("SpaceMan");
        }, () =>
        {
            List<Vector2> outputs = new List<Vector2>();
            outputs.Add(new Vector2(24f, -11f));
            return outputs;
        }, visualElement);


        name = "killallbugs";
        visualElement = await createVisualElement(name);
        objectives[name] = new Objective(name, () =>
        {
            foreach (Transform enemy in enemies.transform)
            {
                if (enemy.gameObject.tag == "Bug")
                {
                    return false;
                }
            }
            return true;
        }, () =>
        {
            List<Vector2> outputs = new List<Vector2>();
            foreach (Transform enemy in enemies.transform)
            {
                if (enemy.gameObject.tag == "Bug")
                {
                    outputs.Add((Vector2)enemy.transform.position);
                }
            }
            return outputs;
        }, visualElement);


        name = "getyourgun";
        GameObject spaceMan = GameObject.Find("SpaceMan");
        GameObject spaceManHand = spaceMan.transform.Find("Hand").gameObject;
        GameObject gun = GameObject.Find("Gun");
        visualElement = await createVisualElement(name);
        objectives[name] = new Objective(name, () =>
        {
            if (spaceManHand.transform.childCount == 1)
            {
                return true;
            }
            return false;
        }, () =>
        {
            List<Vector2> outputs = new List<Vector2>();
            outputs.Add((Vector2)gun.transform.position);
            return outputs;
        }, visualElement);


        name = "nocurrentobjective";
        visualElement = await createVisualElement(name);
        objectives[name] = new Objective(name, () =>
        {
            return false;
        },null, visualElement);


        name = "flytoasteroid";
        GameObject asteroid1Trigger = GameObject.Find("Asteroid1Trigger");
        PlanetTrigger asteroid1TriggerScript = asteroid1Trigger.GetComponent<PlanetTrigger>();
        visualElement = await createVisualElement(name);
        objectives[name] = new Objective(name, () =>
        {
            return asteroid1TriggerScript.checkIfOverlapping("SpaceMan");
        }, () =>
        {
            List<Vector2> outputs = new List<Vector2>();
            outputs.Add(new Vector2(-15f, -17f));
            return outputs;
        }, visualElement);
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


    public Dictionary<string,Objective> getObjectives()
    {
        return objectives;
    }
}
