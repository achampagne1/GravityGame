using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class GunController : ItemController
{

    //vectors
    private Vector2 forceBuffer = new Vector2(0, 0);
    private Vector3 shootDirection = Vector3.zero;

    //object creation
    private GameObject bulletObject;
    private Transform playerBody;
    private HandController handController;
    private Timer throwTimer;
    private Animator animator;


    //private variables
    private bool parented = false;
    private bool facingLeft = false;
    private bool parentLatch = true;

    public void Start()
    {
        throwTimer = new Timer(.25f); //this is to make sure the player doesnt immidietly grab the item when it is thrown
        Physics2D.IgnoreLayerCollision(9, 13, true);
        Physics2D.IgnoreLayerCollision(11, 13, true);
        parented = transform.parent != null; //parenting will need to be moved to item controller if more items are added
        bulletObject = transform.GetChild(0).gameObject; //the bullet is the first child object of the gun
        if (parented)
        {
            GameObject temp = transform.parent.gameObject.transform.parent.gameObject; //this is the gameObject of the character
            handController = transform.parent.GetComponent<HandController>();
            playerBody = temp.GetComponent<Transform>(); //I want to get rid of the need for the player body and jsut ude the hand but idk how
        }

        try
        {
            animator = GetComponent<Animator>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        calculateItemStart();   
    }

    public void Update()
    {
        parented = transform.parent != null; //parenting will need to be moved to item controller if more items are added
        calculateItemUpdate();
        if (parented)
        {
            if (!parentLatch)
            {
                GameObject temp = transform.parent.gameObject.transform.parent.gameObject; //this is the gameObject of the character
                handController = transform.parent.GetComponent<HandController>();
                playerBody = temp.GetComponent<Transform>(); //I want to get rid of the need for the player body and jsut ude the hand but idk how
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            floatFlag = false;
            gravityAffected = false;    
            orientToGravity = false;
            facingLeft = handController.getFacingLeft();
            /**if (Mouse.current.leftButton.wasPressedThisFrame) //I would like to have it come from the hand controller but thats laggy
            {
                shootWrapper();
            }**/
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (parentLatch)
            {
                rb.AddForce(forceBuffer, ForceMode2D.Impulse);
                handController = null;
                playerBody = null;
            }
            floatFlag=true; 

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
        shoot(transform.position + transform.rotation * offset, shootDirection);
    }

    private void shoot(Vector3 location, Vector3 direction)
    {
        GameObject ShotBullet = Instantiate(bulletObject, location, Quaternion.identity);
        ShotBullet.transform.localScale = new Vector3(.075f, .075f, .075f);
        ShotBullet.GetComponent<BulletController>().newInstance(direction);
        ShotBullet.GetComponent<BulletController>().Start();
    }

    private static Vector3 getMouseDirection(Vector3 mousePosition, Quaternion playerRotation)
    {
        //This function was written by chat GPT
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(mousePosition.x, mousePosition.y) - screenCenter;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 direction3D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0f);
        Vector3 rotatedDirection = playerRotation * direction3D;
        return new Vector2(rotatedDirection.x, rotatedDirection.y).normalized;
    }

    public void setForceBuffer(Vector2 force)
    {
        calculateGravity();
        Physics2D.IgnoreLayerCollision(13, 14, true);
        throwTimer.startTimer();
        float rotatedX = -gravityDirection.x;
        float rotatedY = -gravityDirection.y;

        forceBuffer = new Vector2(rotatedX * force.x, rotatedY * force.y);
    }

    public void setParent(GameObject parent)
    {
        transform.SetParent(parent.transform); //slightly different method
        transform.rotation = parent.transform.rotation;
        if (parent.gameObject.GetComponent<HandController>().getFacingLeft()!=facingLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            transform.localPosition = new Vector3(-2.5f, -1f, 0f); //for setting location of gun in hand
            facingLeft = !facingLeft;
        }
        else
            transform.localPosition = new Vector3(2.5f, 1f, 0f);
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