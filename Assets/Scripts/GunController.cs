using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : ItemController
{


    //object creation
    GameObject bulletObject;
    Transform playerBody;
    CharacterController characterController;

    //private variables
    private bool parented = false;

    public void Start()
    {
        bulletObject = GameObject.Find("Bullet");
        GameObject temp = transform.parent.gameObject.transform.parent.gameObject; //this is the gameObject of the character
        characterController = temp.GetComponent<CharacterController>();
        playerBody = temp.GetComponent<Transform>();
        calculateItemStart();   
    }

    public void Update()
    {
        parented = transform.parent != null; //parenting will need to be moved to item controller if more items are added

        calculateItemUpdate();
        if (parented && characterController.getClick())
        {
            Vector3 offset = new Vector3(.5f, .25f, 0);
            offset.y = offset.y * (characterController.getFacingLeft() ? -1 : 1);
            shoot(transform.position + transform.rotation * offset, getMouseDirection(Input.mousePosition, playerBody.rotation));
        }
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
}