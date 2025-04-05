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
        objectives = new Stack<Objective>(2); //I am only working with 2 objectives at the moment.
    }

    public async Task initializeObjectives()
    {
        Objective objective2 = new Objective("killenemy", () =>
        {
            return false; //temporary
        }, null);

        await createVisualElement("killenemy", (VisualElement visualElement) =>
        {
            if (visualElement != null)
            {
                objective2.visualElement = visualElement;  // Update the Objective with the loaded VisualElement
                objectives.Push(objective2);  // Push to the stack once the element is ready
            }
            else
            {
                Debug.LogError("Failed to create VisualElement for 'killenemy_0'.");
            }
        });
    }

    private async Task createVisualElement(string name, Action<VisualElement> onComplete)
    {
        VisualElement visualElement = new VisualElement();

        // Use await to asynchronously load the texture
        var handle = Addressables.LoadAssetAsync<Texture2D>("Assets/Art/UI/WaypointText/" + name + ".png");

        // Wait until the operation completes
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            visualElement.style.backgroundImage = new StyleBackground(handle.Result);
            onComplete?.Invoke(visualElement);  // Return when image loads
        }
        else
        {
            Debug.LogError("Failed to load Addressable texture.");
            onComplete?.Invoke(null);  // Return null if loading fails
        }
    }


    public Objective getNextObjective()
    {
        return objectives.Pop();
    }
}
