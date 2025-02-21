using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceManController : CharacterController
{
    //object creation
    GunWrapper gunWrapper;

    //game variables
    bool enemyCollideFlag = false;

    void Start()
    {
        calculateCharacterStart();
        setMaxHealth(10f);
        gunWrapper = new GunWrapper();
        gunWrapper.Start();
    }

    void FixedUpdate()
    {
        setMovement(inputSystemToGetAxis());
        setJump(Keyboard.current.spaceKey.isPressed);
        calculateCharacterUpdate();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            gunWrapper.shoot(transform.position, getMouseDirection(Input.mousePosition, transform.rotation));
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "SpaceZombie(Clone)" && enemyCollideFlag == false)
        {
            //Vector2 forceVector = new Vector2(-5f,0f);
            //addForceLocal(forceVector);
            setHealth(getHealth() - 1f);
            UIHandler.instance.setHealthValue(getHealth());
            enemyCollideFlag = true;
        }
        else if (collision.gameObject.name != "SpaceZombie(Clone)" && enemyCollideFlag == true)
            enemyCollideFlag = false;

        if (collision.gameObject.name == "MedPack")
        {
            setHealth(10f);
            UIHandler.instance.setHealthValue(getHealth());
        }
            

    }

    int inputSystemToGetAxis()
    {
        if (Keyboard.current.aKey.isPressed)
            return -1;
        if (Keyboard.current.dKey.isPressed)
            return 1;
        else
            return 0;
    }

}