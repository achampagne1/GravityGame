using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePersonController : CharacterController
{
    //object creation
    private Animator animator; //movved
    private SpriteRenderer jetPackFlame;
    protected HandController handController;
    protected Timer hoverTimer;

    //public game variables;
    public float jetPackForce = 30f;

    //private game variables
    private float maxFuel = 100f; // Maximum fuel capacity
    protected bool throwItem = false;
    private bool hoverFlag = false;

    //protected game variables
    protected float currentFuel = 100f;

    //vectors
    private Vector2 hover = new Vector2(0, 0);



    public void calculateSpacePersonStart()
    {    
        foreach(Transform child in transform)
        {
            if (child.name == "JetPackFlame")
                jetPackFlame = child.gameObject.GetComponent<SpriteRenderer>();
            if (child.name == "Hand")
                handController = child.gameObject.GetComponent<HandController>();
        }
        calculateCharacterStart();
    }

    public void calculateSpacePersonUpdate()
    {
        calculateJetPackHover();

        rb.AddForce(hover);

        calculateCharacterUpdate();  
    }

    public virtual void Update()
    {
        int ham = 0;
        //basically an abstract funciton
    }

    private void calculateJetPackHover() //might change this to space man only
    {
        rotatedX = -gravityDirection.x;
        rotatedY = -gravityDirection.y;
        if (space && groundTimer.checkTimer() && currentFuel > 0)
        {
            hoverFlag = true;
            Color color = jetPackFlame.color;
            color.a = 1.0f; // Set alpha (0 = transparent, 1 = opaque)
            jetPackFlame.color = color;
            useFuel();
        }

        if (!space || currentFuel == 0)
        {
            hoverFlag = false;
            Color color = jetPackFlame.color;
            color.a = 0.0f; // Set alpha (0 = transparent, 1 = opaque)
            jetPackFlame.color = color;
        }

        hover = hoverFlag ? new Vector2(rotatedX * jetPackForce, rotatedY * jetPackForce) : Vector2.zero;
    }

    private void useFuel()
    {
        float fuelConsumptionRate = 100f; // Fuel units per second
          currentFuel -= fuelConsumptionRate * Time.deltaTime; // Decrease fuel over time

        if (currentFuel < 0)
            currentFuel = 0; // Prevent negative fuel
    }

    protected override IEnumerator die()
    {
        handController.throwItem();
        handController.destroyWrapper();
        yield return base.die();
    }

    public bool getThrow()
    {
        return throwItem;
    }
}
