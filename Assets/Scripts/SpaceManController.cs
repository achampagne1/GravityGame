using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceManController : CharacterController
{

    //game variables
    bool enemyCollideFlag = false;

    void Start()
    {
        calculateCharacterStart();
        setMaxHealth(10f);
    }

    void FixedUpdate()
    {
        setMovement(inputSystemToGetAxis());
        setJump(Keyboard.current.spaceKey.isPressed);
        calculateCharacterUpdate();
    }

    private void Update()
    {
       
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