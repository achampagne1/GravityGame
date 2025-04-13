using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZombieController : CharacterController
{
    //object creation
    RandomTimer pauseDuration = new RandomTimer();
    RandomTimer moveDuration = new RandomTimer();
    Timer timer = new Timer(3f);
    Timer shootTimer = new Timer(3f);

    //public variables
    public bool first = false;
    public bool movementToggle = true;
    public int normalState = 0;

    //game variables
    private int moveInput = 0;
    private bool pause = false;
    private bool following = false;
    private bool attackLatch = false;
    private Vector3 playerDirection = new Vector3(0f, 0f, 0f);

    void Start()
    {
        calculateCharacterStart();
        pauseDuration.create(.1f, 1f);
        moveDuration.create(1f, 4f);

        timer.startTimer();
        shootTimer.startTimer(); //shoot timer must be started so that the enemey is ready when it first sees the player
    }

    public void FixedUpdate()
    {
        if (!first)
        {
            if (movementToggle && !dead)
            {
                if (detectPlayer())
                {
                    attackPlayer();
                }
                else
                {
                    if (normalState == 0)
                        moveInput = 0;
                    else if (normalState == 1)
                        patrol();
                    else
                        randomMovement();
                }
                setMovement(moveInput);
                setOrientation(moveInput);
            }
            calculateCharacterUpdate();
        }
    }

    private void randomMovement()
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

    private void patrol()
    {
        if(moveInput == 0)
        {
            moveInput = 1;
        }

        handController.setInputDirection(transform.rotation * new Vector3((float)moveInput, 0f, 0f));


        if (timer.checkTimer()||detectLedge()||wallInFrontVar==moveInput)
        {
            moveInput = moveInput * -1;
            timer.startTimer();
        }

    }

    private void attackPlayer() //TODO: have the timer automatically reset if the player gets out of detection range
    {
        if (shootTimer.checkTimer())
        {
            handController.setInputDirection(playerDirection);
            StartCoroutine(clusterShot());
            shootTimer.startTimer();
        }
        moveInput = 0;
    }

    private IEnumerator clusterShot()
    {
        for (int i = 0; i < 3; i++)
        {
            handController.useHand();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private bool detectPlayer()
    {
        /**int layerMask = ~((1 << LayerMask.NameToLayer("items"))
                 | (1 << LayerMask.NameToLayer("bullet"))
                 | (1 << LayerMask.NameToLayer("Ignore Raycast")));
        add this later**/
        for (int i = 0; i<60; i++)
        {
            float angle =  (getCharacterOrientation()+30 - i +(System.Convert.ToSingle(getFacingLeft()) *180)) % 360;
            Vector2 temp = new Vector2(Mathf.Cos(angle * Mathf.PI / 180), Mathf.Sin(angle * Mathf.PI / 180));
            RaycastHit2D[] lookForPlayer = Physics2D.RaycastAll(transform.position,temp, 100f);
            foreach (RaycastHit2D hit in lookForPlayer)
            {
                if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 15)
                    break;
                if (hit.collider.gameObject != gameObject&& hit.collider.gameObject.layer == 9)
                {
                    playerDirection = new Vector3(temp.x, temp.y, 0f);
                    return true;
                }
            }
        }
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
            setHealth(health - 1f);
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

    protected override void die()
    {
        base.die();
        moveInput = 0;
        setMovement(moveInput);
        handController.throwItem();
        explodeController.trigger(); //this will need to get moved to character controller once art for spaceman is done
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.0f; // Opacity from 0 (transparent) to 1 (fully opaque)
        sr.color = c;

    }
}   