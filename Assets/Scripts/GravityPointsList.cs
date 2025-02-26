using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPointsList : MonoBehaviour
{
    public List<GameObject> gravityPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            gravityPoints.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
