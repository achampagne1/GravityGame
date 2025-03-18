using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedPackController : ItemController
{
    //object creation

    // Start is called before the first frame update
    public void Start()
    {
        calculateItemStart();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        calculateItemUpdate();
    }

}
