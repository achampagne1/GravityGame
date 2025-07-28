using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePersonController : CharacterController
{
    //object creation
    private SpriteRenderer jetPackFlame;
    protected HandController handController;
    private AudioSource jetPackAudioSource;
    protected Timer hoverTimer;

    //public game variables;
    [SerializeField] private float jetPackForce = 30f;
    [SerializeField] private float groundSmokeTime = 2f;
    [SerializeField] private GameObject landingSmoke;


    //private game variables
    private float maxFuel = 100f; // Maximum fuel capacity
    protected bool throwItem = false;
    private bool hoverFlag = false;
    private bool smokeLatch = false;

    //protected game variables
    protected float currentFuel = 100f;

    //vectors
    private Vector2 hover = new Vector2(0, 0);



    public void calculateSpacePersonStart()
    {
        try
        {
            jetPackAudioSource = GetComponents<AudioSource>()[1];
        }
        catch
        {
            Debug.LogError("no attached audio source");
        }

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
        smokeLatch = groundStopWatch.getElapsedTime() > groundSmokeTime;

        calculateJetPackHover();

        rb.AddForce(hover);

        calculateCharacterUpdate();
        if (isGrounded && smokeLatch)
        {
            Vector3 footPosition = transform.position - transform.up * (heightObject / 2f);
            GameObject smoke = Instantiate(landingSmoke, footPosition, transform.rotation);
        }
        smokeLatch = false;
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
        if (space && groundStopWatch.getElapsedTime()>0.4f && !hoverFlag&& currentFuel > 0)
        {
            jetPackAudioSource.Play();
            hoverFlag = true;
            Color color = jetPackFlame.color;
            color.a = 1.0f; // Set alpha (0 = transparent, 1 = opaque)
            jetPackFlame.color = color;
        }

        if (!space || currentFuel == 0)
        {
            jetPackAudioSource.Stop();
            hoverFlag = false;
            Color color = jetPackFlame.color;
            color.a = 0.0f; // Set alpha (0 = transparent, 1 = opaque)
            jetPackFlame.color = color;
        }

        if (hoverFlag)
            useFuel();
        hover = hoverFlag ? new Vector2(rotatedX * jetPackForce, rotatedY * jetPackForce) : Vector2.zero;
    }

    private void useFuel()
    {
        float fuelConsumptionRate = 10f; // Fuel units per second
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

    public float getCurrentFuel()
    {
        return currentFuel;
    }

    public bool getThrow()
    {
        return throwItem;
    }
}
