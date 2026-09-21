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
    [SerializeField] private float relaxTime = 3f;
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
        }

        if(handController == null)
        {
            Debug.Log("no hand found on Space Person Controller");
        }

        base.Start();

        handController.itemSuccessfullyUsedOnce += onItemSucessfullyUsedOnce;
        handController.itemSuccessfullyUsedHold += onItemSucessfullyUsedHold;
        handController.itemSuccessfullyUsedRelease += onItemSuccessfullyUsedRelease;
        handController.changeInItemEffect += onChangeInItemEffect;
        onChangeInItemEffect(handController.getItemParentedEffect());
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

        if(!handController.getHolding())
        {
            characterState = CHARACTERSTATE.IDLE;
        }
        else if (handController.getHolding())
        {
            if(handController.getTimeLastUsedDiff() > relaxTime)
            {
                characterState = CHARACTERSTATE.IDLE;
            }
            else
            {
                characterState = CHARACTERSTATE.AIMING;
            }
        }

        smokeLatch = false;
        holdingLatch = handController.getHolding();
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

    private void onItemSucessfullyUsedOnce()
    {
        foreach (CharacterProp prop in characterProps)
        {
            prop.useParentedEffect();
        }
    }

    private void onItemSucessfullyUsedHold()
    {

    }

    private void onItemSuccessfullyUsedRelease()
    {

    }

    private void onChangeInItemEffect(IParentedEffect itemEffect)
    {
        foreach(CharacterProp prop in characterProps)
        {
            if (itemEffect == null)
            {
                prop.setItemParentedEffect(null);
                continue;
            }

            Type effectType = itemEffect.GetType();
            IParentedEffect newItemEffect = (IParentedEffect)prop.gameObject.AddComponent(effectType);
            newItemEffect.copyData(itemEffect);

            prop.setItemParentedEffect(newItemEffect);
        }
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
