using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : CharacterController
{
    [SerializeField] float persistanceAfterDeath = 5f;
    [SerializeField] float jumpAngle = 20f;
    [SerializeField] float jumpMagnitude = 10f;
    [SerializeField] float pounceCooldownTime = 5f;
    private bool pounceRunning = false;
    private RandomTimer blinkTimer;

    // Start is called before the first frame update
    void Start()
    {
        blinkTimer = new RandomTimer(3, 5);
        calculateCharacterStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!pounceRunning)
            StartCoroutine(jump());
        calculateCharacterUpdate();
    }

    private IEnumerator jump()
    {
        pounceRunning = true;       
        forceLocalAdded = true;

        Vector2 localDir = HelperFunctions.angleToDirection(jumpAngle);
        float zRotation = transform.eulerAngles.z;
        Quaternion rotation = Quaternion.Euler(0, 0, zRotation);
        Vector2 worldDir = rotation * localDir;
        Vector2 finalJump = worldDir * jumpMagnitude;
        forceLocal = finalJump;
        //the timing of this might need to be redone one day
        yield return new WaitUntil(()=>isGrounded);
        yield return new WaitForSeconds(pounceCooldownTime);
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
