using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : ObjectController
{
    //object creation

    //vectors


    // Start is called before the first frame update
    void Start()
    {
        calculateStart();
    }

    // Update is called once per frame
    void Update()
    {
        calculateUpdate();
    }
}
