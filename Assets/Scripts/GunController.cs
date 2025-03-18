using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : ItemController
{

    //vectors
    private Vector2 forceBuffer = new Vector2(0, 0);

    //object creation
    private GameObject bulletObject;
    private Transform playerBody;
    private HandController handController;


    //private variables
    private bool parented = false;
    private bool facingLeft = false;
    private bool parentLatch = true;

    public void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 13, true);
        bulletObject = transform.GetChild(0).gameObject; //the bullet is the first child object of the gun
        GameObject temp = transform.parent.gameObject.transform.parent.gameObject; //this is the gameObject of the character
        handController = transform.parent.GetComponent<HandController>();
        playerBody = temp.GetComponent<Transform>(); //I want to get rid of the need for the player body and jsut ude the hand but idk how
        calculateItemStart();   
    }

    public void Update()
    {
        parented = transform.parent != null; //parenting will need to be moved to item controller if more items are added

        calculateItemUpdate();
        if (parented)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            floatFlag = false;
            gravityAffected = false;    
            orientToGravity = false;
            facingLeft = handController.getFacingLeft();
            if (handController.getClick())
            {
                Vector3 offset = new Vector3(.5f, .25f, 0);
                offset.y = offset.y * (facingLeft ? -1 : 1);
                shoot(transform.position + transform.rotation * offset, getMouseDirection(Input.mousePosition, playerBody.rotation));
            }
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            if(parentLatch)
                rb.AddForce(forceBuffer,ForceMode2D.Impulse);
            floatFlag=true; 

        }
        parentLatch = parented;
    }

    public void shoot(Vector3 location, Vector3 direction)
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

    public bool getFacingLeft()
    {
        return facingLeft;
    }

    public void setForceBuffer(Vector2 force)
    {
        forceBuffer = force;
    }
}