using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class BugController : CharacterController
{
    [SerializeField] float persistanceAfterDeath = 5f;
    [SerializeField] float jumpAngle = 20f;
    [SerializeField] float jumpMagnitude = 10f;
    [SerializeField] float pounceCooldownTime =.1f;
    private bool pounceRunning = false;
    private bool pause = false;
    private int moveInput = 0;
    private Vector3 playerDirection = new Vector3(0f, 0f, 0f);

    //object creation
    private RandomTimer pauseDuration;
    private RandomTimer moveDuration;
    private RandomTimer blinkTimer;

    // Start is called before the first frame update
    void Start()
    {
        blinkTimer = new RandomTimer(3, 5);
        pauseDuration = new RandomTimer(.1f, 1f);
        moveDuration = new RandomTimer(1f, 4f);
        calculateCharacterStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movementToggle && !dead)
        {
            playerDirection = EnemyAssistant.detectPlayer(getFacingLeft(), gameObject);
            if (playerDirection != new Vector3(0f, 0f, 1f))
            {
                Vector2 temp = HelperFunctions.rotateVector(new Vector2(playerDirection.x,playerDirection.y),-transform.eulerAngles.z);
                if (!pounceRunning)
                {
                    StartCoroutine(jump(temp.x<0));
                }
            }
            else
            {
                randomMovement();
            }
            setOrientation(moveInput);
        }
        calculateCharacterUpdate();
    }

    private void randomMovement()
    {
        if (moveDuration.checkTimer() && pause) //move state
        {
            pause = false;
            moveInput = UnityEngine.Random.Range(-1, 2);
            moveDuration.resetTimer();
            pauseDuration.resetTimer();
        }
        else if (pauseDuration.checkTimer() && !pause) //!move state
        {
            pause = true;
            moveInput = 0;
            moveDuration.resetTimer();
            pauseDuration.resetTimer();
        }

        if (!pounceRunning&&moveInput!=0)
        {
            StartCoroutine(jump(moveInput==-1));
        }

    }

    private IEnumerator jump(bool facingLeft)
    {
        pounceRunning = true;       
        forceLocalAdded = true;
        //th ternary operation allows for jumping left
        Vector2 localDir = HelperFunctions.angleToDirection(facingLeft? 180-jumpAngle:jumpAngle);
        float zRotation = transform.eulerAngles.z;
        Quaternion rotation = Quaternion.Euler(0, 0, zRotation);
        Vector2 worldDir = rotation * localDir;
        Vector2 finalJump = worldDir * jumpMagnitude;
        forceLocal = finalJump;
        //the timing of this might need to be redone one day
        yield return new WaitForSeconds(pounceCooldownTime);
        yield return new WaitUntil(()=>isGrounded);
        pounceRunning = false;
        forceLocalAdded = false;
        yield return null;
    }

    protected override IEnumerator die()
    {
        setMovement(0);
        yield return base.die();
        yield return new WaitForSeconds(persistanceAfterDeath);
        Destroy(gameObject);
    }

    protected override void determineAnimation()
    {
        try
        {
            if (isGrounded)
            {
                animator.SetBool("Airborn", false);
                if (blinkTimer.checkTimer())
                {
                    //this chunk of code allows the slime to blink twie sometimes
                    if(UnityEngine.Random.Range(0, 5)==0)
                        StartCoroutine(blinkTwice());
                    else
                        animator.SetTrigger("Blink");
                    blinkTimer.resetTimer();
                }
            }
            else
            {
                animator.SetBool("Airborn", true);
                animator.SetBool("Up", up);
            }
        }
        catch (Exception e)
        {
            bool ham = false;
        }

        IEnumerator blinkTwice()
        {
            animator.SetTrigger("Blink");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("SlimeBlink"));
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            yield return new WaitForSeconds(.1f);
            animator.SetTrigger("Blink");
            yield return null;
        }
    }
}
