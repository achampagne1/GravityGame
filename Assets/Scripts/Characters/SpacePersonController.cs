using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class SpacePersonController : CharacterController
{
    //object creation
    protected HandController handController;
    private AudioSource jetPackAudioSource;
    protected Timer hoverTimer;

    //public game variables;
    [SerializeField] private float jetPackForce = 30f;
    [SerializeField] private float groundSmokeTime = 2f;
    [SerializeField] private float footOffset = .1f;
    [SerializeField] private GameObject landingSmoke;
    [SerializeField] private GameObject jetPackFlame;
    [SerializeField] private Vector2 flameOffset = new Vector2(0, 0);
    [SerializeField] private Vector2 maxVisorAngle = new Vector2(-45, 45);
    [SerializeField] private Vector2 maxJetPackAngle = new Vector2(-45, 45);


    //private game variables
    private float maxFuel = 100f; // Maximum fuel capacity
    protected bool throwItem = false;
    private bool hoverFlag = false;
    private bool smokeLatch = false;
    private GameObject jetPackFlameClone;
    private Transform visor;
    private Transform jetPack;
    private Vector3 originalVisorPos;
    private Vector3 originalJetPackPos;
    private Quaternion originalVisorRot;
    private Quaternion originalJetPackRot;
    private bool holdingLatch = false;

    //protected game variables
    protected float currentFuel = 100f;
    protected Vector2 lookingDirection = Vector2.zero;

    //vectors
    private Vector2 hover = new Vector2(0, 0);



    public override void Start()
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
            if (child.name == "Hand")
                handController = child.gameObject.GetComponent<HandController>();
            else if (child.name == "Visor")
                visor = child;
            else if (child.name == "JetPack")
                jetPack = child;
        }
        originalVisorPos = visor.localPosition;
        originalJetPackPos = jetPack.localPosition;
        originalVisorRot = visor.localRotation;
        originalJetPackRot = jetPack.localRotation;


        base.Start();
    }

    public override void FixedUpdate()
    {
        smokeLatch = groundStopWatch.getElapsedTime() > groundSmokeTime;

        calculateJetPackHover();

        rb.AddForce(hover);

        base.FixedUpdate();

        if (isGrounded && smokeLatch)
        {
            Vector3 footPosition = transform.position - transform.up * (heightObject / 2f - footOffset);
            GameObject smoke = Instantiate(landingSmoke, footPosition, transform.rotation);
            smoke.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        if(holdingLatch != handController.getHolding() && !handController.getHolding())
        {
            visor.localPosition = originalVisorPos;
            jetPack.localPosition = originalJetPackPos;
            visor.localRotation = originalVisorRot;
            jetPack.localRotation = originalJetPackRot;
        }

        if (handController.getHolding())
            angleJetPackAndVisor();

        smokeLatch = false;
        holdingLatch = handController.getHolding();
    }

    private void angleJetPackAndVisor()
    {
        int facingLeftInt = facingLeft ? -1 : 1;
        Vector2 localLookingDirection = transform.InverseTransformDirection(lookingDirection * facingLeftInt);
        float angle = Mathf.Atan2(localLookingDirection.y, localLookingDirection.x) * Mathf.Rad2Deg;
        float visorAngle;
        float jetPackAngle;

        if (!facingLeft)
        {
            visorAngle = Mathf.Clamp(angle, maxVisorAngle.y, maxVisorAngle.x);
            jetPackAngle = Mathf.Clamp(angle, maxJetPackAngle.y, maxJetPackAngle.x);
        }
        else
        {
            visorAngle = Mathf.Clamp(angle, -maxVisorAngle.x, -maxVisorAngle.y);
            jetPackAngle = Mathf.Clamp(angle, -maxJetPackAngle.x, -maxJetPackAngle.y);
        }

        Quaternion lookRotationVisor = Quaternion.Euler(0f, 0f, visorAngle * facingLeftInt);
        Quaternion lookRotationJetPack = Quaternion.Euler(0f, 0f, jetPackAngle * facingLeftInt);

        visor.localPosition = lookRotationVisor * originalVisorPos;
        jetPack.localPosition = lookRotationJetPack * originalJetPackPos;

        visor.localRotation = lookRotationVisor;
        jetPack.localRotation = lookRotationJetPack;
    }

    public override void triggerLogic(Collider2D trigger)
    {
        ItemController itemController = trigger.gameObject.GetComponent<ItemController>();
        if (itemController != null && itemController.getGrabable() && !handController.getHolding())
            handController.setChild(trigger.transform);

        base.triggerLogic(trigger);
    }

    public virtual void Update()
    {
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
            jetPackFlameClone = Instantiate(jetPackFlame);
            jetPackFlameClone.transform.parent = gameObject.transform;
            jetPackFlameClone.transform.rotation = transform.rotation;
            jetPackFlameClone.transform.localPosition = new Vector3(flameOffset[0],flameOffset[1], transform.position.z);
        }

        if (!space || currentFuel == 0)
        {
            jetPackAudioSource.Stop();
            hoverFlag = false;
            Destroy(jetPackFlameClone);
        }

        if (hoverFlag)
            useFuel();
        hover = hoverFlag ? new Vector2(rotatedX * jetPackForce, rotatedY * jetPackForce) : Vector2.zero; //avoid new
    }

    private void useFuel()
    {
        float fuelConsumptionRate = 10f;
          currentFuel -= fuelConsumptionRate * Time.deltaTime; 

        if (currentFuel < 0)
            currentFuel = 0; 
    }

    protected override IEnumerator die()
    {
        //handController.throwItem(); //redo this
        /*handController.destroyWrapper();*/
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
