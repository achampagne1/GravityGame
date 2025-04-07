using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZombieController : CharacterController
{
    //object creation
    RandomTimer pauseDuration = new RandomTimer();
    RandomTimer moveDuration = new RandomTimer();

    //public variables
    public bool first = false;

    //game variables
    int moveInput = 0;
    bool pause = false;
    bool following = false;
    public bool movementToggle = true;

    void Start()
    {
        calculateCharacterStart();
        pauseDuration.create(.1f, 1f);
        moveDuration.create(1f, 4f);
    }

    void FixedUpdate()
    {
        if (!first)
        {
            if (movementToggle) //for debugging purposes
            {
                if (detectPlayer())
                {
                    if (getFacingLeft())
                        moveInput = -1;
                    else
                        moveInput = 1;
                    following = true;
                }
                else
                    randomMovement();
                setMovement(moveInput);
                setOrientation(moveInput);
            }
            calculateCharacterUpdate();
            if (getHealth() == 0)
                Destroy(this.gameObject);
        }
    }

    public override void Update(){
        if (getFacingLeft())
            handController.setInputDirection(transform.rotation*new Vector3(-1f, 0f, 0f));
        else
            handController.setInputDirection(transform.rotation*new Vector3(1f, 0f, 0f));
    }

    void randomMovement()
    {
        if (moveDuration.checkTimer()&&pause) //move state
        {
            pause = false;
            moveInput = UnityEngine.Random.Range(-1, 2);
            moveDuration.resetTimer();
            pauseDuration.resetTimer();
        }
        else if (pauseDuration.checkTimer()&&!pause) //!move state
        {
            pause = true;
            moveInput = 0;
            moveDuration.resetTimer();
            pauseDuration.resetTimer();
        }
    }

    bool detectPlayer()
    {
        /**for (int i = 0; i<60; i++)
        {
            float angle =  (getCharacterOrientation()+30 - i +(System.Convert.ToSingle(getFacingLeft()) *180)) % 360;
            Vector2 temp = new Vector2(Mathf.Cos(angle * Mathf.PI / 180), Mathf.Sin(angle * Mathf.PI / 180));
            RaycastHit2D lookForPlayer = Physics2D.Raycast(getCharacterCollider().bounds.center,temp, 100f, LayerMask.GetMask("player"));
            RaycastHit2D lookForObstacles = Physics2D.Raycast(getCharacterCollider().bounds.center, temp, 100f, LayerMask.GetMask("Default"));
            if (lookForPlayer.collider != null&& lookForObstacles.collider == null)
                return true;
        }**/
        return false;
    }

    public void newInstance()
    {
        first = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            setHealth(getHealth() - 1f);
        }
    }

    private int lookLeftOrRight()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter;

        if (direction.x < 0)
            return -1;
        else
            return 1;
    }
}   