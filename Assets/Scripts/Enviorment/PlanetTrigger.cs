using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTrigger : MonoBehaviour
{
    List<string> overlapping = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        overlapping.Add(trigger.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        overlapping.Remove(trigger.gameObject.name);
    }

    public bool checkIfOverlapping(string name)
    {
        foreach(string nameLocal in overlapping)
        {
            if (name == nameLocal)
                return true;
        }
        return false;
    }
}
