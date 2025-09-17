using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGuy : SpacePersonController
{

    void Start()
    {
        base.Start();
        setMovement(1);
        setOrientation(1);

    }

    public void FixedUpdate()
    {
        handController.setInputDirection(transform.rotation * new Vector3(.7f, -.3f, 0f));
        base.FixedUpdate();
    }
}
