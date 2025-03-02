using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceManController : CharacterController
{
    //object creation 
    private Animator animator;

    //game variables
    bool enemyCollideFlag = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        calculateCharacterStart();
        setMaxHealth(10f);
    }

    void FixedUpdate()
    {
        setMovement(inputSystemToGetAxis());
        setOrientation(lookLeftOrRight());
        setJump(Keyboard.current.spaceKey.isPressed);
        calculateCharacterUpdate();
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
        {
            animator.SetBool("Walk", true);
            return -1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            animator.SetBool("Walk", true);
            return 1;
        }
        else
        {
            animator.SetBool("Walk", false);
            return 0;
        }
    }

    int lookLeftOrRight()
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