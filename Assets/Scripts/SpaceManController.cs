using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceManController : CharacterController
{
    
    //game variables
    bool enemyCollideFlag = false;

    public void Start()
    {
        calculateCharacterStart();
        setMaxHealth(10f);

    }

    public void FixedUpdate()
    {
        setMovement(inputSystemToGetAxis());
        setOrientation(lookLeftOrRight());
        setJump(Keyboard.current.spaceKey.isPressed);
        setThrow(Keyboard.current.qKey.isPressed);
        setClick(Mouse.current.leftButton.wasPressedThisFrame);
        calculateCharacterUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.name == "SpaceZombie(Clone)" || collision.gameObject.name == "SpaceZombie") && enemyCollideFlag == false)
        {
            //Vector2 forceVector = new Vector2(-5f,0f);
            //addForceLocal(forceVector);
            setHealth(getHealth() - 1f);
            UIHandler.instance.setHealthValue(getHealth());
            enemyCollideFlag = true;
        }
        else if ((collision.gameObject.name != "SpaceZombie(Clone)" || collision.gameObject.name != "SpaceZombie") && enemyCollideFlag == true)
            enemyCollideFlag = false;

        if (collision.gameObject.name == "MedPack")
        {
            setHealth(10f);
            UIHandler.instance.setHealthValue(getHealth());
        }
    }

    public int inputSystemToGetAxis()
    {
        if (Keyboard.current.aKey.isPressed)
            return -1;
        if (Keyboard.current.dKey.isPressed)
            return 1;   
        else
            return 0;
    }

    public int lookLeftOrRight()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter;

        if (direction.x < 0)
        {
            return -1;
        }
        if (direction.x >= 0)
        {
            return 1;
        }
        else
            return 0;
    }

}