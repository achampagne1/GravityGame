using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZombieController : SpacePersonController
{
    //object creation
    RandomTimer pauseDuration;
    RandomTimer moveDuration;
    Timer timer = new Timer(3f);
    Timer shootTimer = new Timer(3f);

    //public variables
    public bool first = false;
    public int normalState = 0;
    [SerializeField] float persistanceAfterDeath = 5f;

    //game variables
    private int moveInput = 0;
    private bool pause = false;
    private bool following = false;
    private bool attackLatch = false;
    private Vector3 playerDirection = new Vector3(0f, 0f, 0f);

    void Start()
    {
        calculateSpacePersonStart();
        pauseDuration = new RandomTimer(.1f, 1f);
        moveDuration = new RandomTimer(1f,4f);

        timer.startTimer();
        shootTimer.startTimer(); //shoot timer must be started so that the enemey is ready when it first sees the player
    }

    public void FixedUpdate()
    {
        if (!first)
        {
            if (movementToggle && !dead)
            {
                playerDirection = EnemyAssistant.detectPlayer(getFacingLeft(), gameObject);
                if (playerDirection !=new Vector3(0f,0f,1f))
                {
                    handController.setInputDirection(playerDirection);
                    attackPlayer();
                }
                else
                {
                    handController.setInputDirection(transform.rotation * new Vector3((float)moveInput, 0f, 0f));
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
            calculateSpacePersonUpdate();
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
        if (moveInput == 0)
        {
            moveInput = 1;
        }


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

    protected override IEnumerator die()
    {
        moveInput = 0;
        setMovement(moveInput);
        yield return base.die();
        yield return new WaitForSeconds(persistanceAfterDeath);
        Destroy(gameObject);
    }
}   