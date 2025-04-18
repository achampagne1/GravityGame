using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : CharacterController
{
    // Start is called before the first frame update
    void Start()
    {
        calculateCharacterStart();
    }

    // Update is called once per frame
    void Update()
    {
        calculateCharacterUpdate();
    }
}
