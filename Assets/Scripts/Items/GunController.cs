using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.FilePathAttribute;

public class GunController : ItemController
{
    //public variables
    [SerializeField] float bulletForce = 50.0f;

    //vectors
    private Vector2 forceBuffer = new Vector2(0, 0);
    private Vector3 shootDirection = Vector3.zero;

    //object creation
    private GameObject bulletObject;
    private Transform playerBody;
    private HandController handController;
    private Timer throwTimer;
    private Timer throwTimer2;
    private Animator animator;
    [SerializeField] AudioClip gunshotClip;
    [SerializeField] GameObject bullet;


    //private variables
    private bool parented = false;
    private bool facingLeft = false;
    private bool parentLatch = true;
    private int shotBy = 0;

    public void Start()
    {
        facingLeft = transform.localScale.x < 0;
        Physics2D.IgnoreLayerCollision(9, 13, true);
        Physics2D.IgnoreLayerCollision(11, 13, true);
        Physics2D.IgnoreLayerCollision(13, 13, true);
        throwTimer = new Timer(.25f); //this is to make sure the player doesnt immidietly grab the item when it is thrown

        calculateItemStart();

        parented = transform.parent != null; //parenting will need to be moved to item controller if more items are added
        if (parented)
        {
            parentedFlags();
        }
        else
        {
            notParentedFlags();
        }

        try
        {
            animator = GetComponent<Animator>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    public void FixedUpdate()
    {
        parented = transform.parent != null; //parenting will need to be moved to item controller if more items are added
        
        if (parented)
        {
            if (!parentLatch)
            {
                parentedFlags();
            }
            facingLeft = handController.getFacingLeft();
        }
        else
        {
            if (parentLatch)
            {
                notParentedFlags();
            }
            calculateItemUpdate();
        }
        parentLatch = parented;

        if(throwTimer.checkTimer())
            Physics2D.IgnoreLayerCollision(13, 14, false);
    }

    public void shootWrapper()
    {
        Vector3 offset = new Vector3(.5f, .25f, 0);
        offset.y = offset.y * (facingLeft ? -1 : 1);
        animator.SetTrigger("Shoot");
        GameObject bulletClone= Instantiate(bullet, transform.position + transform.rotation * offset, transform.rotation);
        bulletClone.GetComponent<BulletController>().init(transform.parent.gameObject.layer);
        bulletClone.GetComponent<Rigidbody2D>().AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
        SoundManager.instance.playSound(gunshotClip, transform, 1f);

    }

    public void setForceBuffer(Vector2 force)
    {
        Physics2D.IgnoreLayerCollision(13, 14, true);
        throwTimer.startTimer();
        forceBuffer = force;
    }

    private void parentingHelper()
    {
        GameObject temp = transform.parent.gameObject.transform.parent.gameObject; //this is the gameObject of the character
        GameObject hand = transform.parent.gameObject;
        handController = hand.GetComponent<HandController>();
        shotBy = hand.layer;
        playerBody = temp.GetComponent<Transform>(); //I want to get rid of the need for the player body and jsut ude the hand but idk how
    }

    public void setParent(GameObject parent)
    {
        transform.SetParent(parent.transform); //slightly different method
        transform.localRotation = Quaternion.identity;
        if (parent.gameObject.GetComponent<HandController>().getFacingLeft()!=facingLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            transform.localPosition = new Vector3(-3f, -1f, 0f); //for setting location of gun in hand
            facingLeft = !facingLeft;
        }
        else
            transform.localPosition = new Vector3(3f, 1f, 0f);
    }

    private void parentedFlags() {
        parentingHelper();
        rb.bodyType = RigidbodyType2D.Kinematic;
        floatFlag = false;
        gravityAffected = false;
        orientToGravity = false;
    }

    private void notParentedFlags()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(forceBuffer, ForceMode2D.Impulse);
        forceBuffer = new Vector2(0, 0);
        handController = null;
        playerBody = null;
        shotBy = 2; //ignore raycast layer
        gravityAffected = false;
        orientToGravity = true;
        //floatFlag=true; 
    }

    public bool getFacingLeft()
    {
        return facingLeft;
    }

    public bool getParented()
    {
        return parented;
    }

    public void setShootDirection(Vector3 input)
    {
        shootDirection = input;
    }


}