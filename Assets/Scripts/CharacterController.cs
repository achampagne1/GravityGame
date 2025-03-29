using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class CharacterController : ObjectController{
    //object creation
    private Animator animator;
    protected Timer hoverTimer;

    //public game variables
    public float moveSpeed = 20f;
    public float jumpForce = 11f;
    public float jetPackForce = 30f;

    //private game variables
    private float horizontalInput = 0;
    private float direcitonInput = 0;
    private float rotatedX = 0;
    private float rotatedY = 0;
    private float jumpMagnitude = 0;
    private float maxHealth = 3f; //default max health is 3
    private float maxFuel = 100f; // Maximum fuel capacity
    private bool space = false;
    private bool facingLeft = false;
    private bool timerFlag = false;
    private bool throwItem = false;
    private bool click = false;
    private bool hoverFlag = false;

    //protected game variables
    protected float currentFuel = 100f;
    protected float health = 0f;

    //vectors
    private Vector2 moveDirection = new Vector2(0, 0);
    private Vector2 jump = new Vector2(0, 0);
    private Vector2 hover = new Vector2(0, 0);
    private Vector2 previousV = new Vector2(0, 0);
    private Vector2 drag = new Vector2(0, 0);
    private Vector2 previousMove = new Vector2(0, 0);
    private Vector2 jumpExtraction = new Vector2(0, 0);
    private Vector2 additionalForce = new Vector2(0, 0);



    public void calculateCharacterStart()
    {
        health = maxHealth;
        calculateStart();      
        try
        {
            animator = GetComponent<Animator>();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }

    }

    public void calculateCharacterUpdate()
    {
        turnLeftRight();
        determineAnimation();

        calculateJump();
        calculateJetPackHover();
        calculateMovement();
        calculateDrag();

        rb.AddForce(jump, ForceMode2D.Impulse);
        rb.AddForce(hover);

        rb.AddForce(moveDirection, ForceMode2D.Impulse);
        rb.AddForce(drag, ForceMode2D.Impulse); //drag is needed because negate the old velcotiy so you can account for hte new agnel and recalculate

        //what this line does is if the player is in the air, it automatically adjusts its jump arc to follow gravity
        if (!isGrounded)
            rb.velocity += -jumpExtraction + jumpMagnitude * -gravityDirection; 
        calculateUpdate();  

        previousV = -rb.velocity;
        previousMove = -moveDirection;
    }

    public void addForceLocal(Vector2 force)
    {
        float angle = Vector2.SignedAngle(gravityDirection, force);
        additionalForce = force;
    }

    private int wallInFront()
    {
        int layerMask = LayerMask.GetMask("Default", "enemy", "player","Platforms"); //how can this be done better
        int currentLayer = gameObject.layer;
        int finalMask = layerMask & ~(1 << currentLayer);

        Vector2 left = Physics2D.Raycast(transform.position,-Vector2.Perpendicular(gravityDirection), heightObject + .2f, finalMask).normal;
        Vector2 right = Physics2D.Raycast(transform.position,Vector2.Perpendicular(gravityDirection), heightObject + .2f, finalMask).normal;
        float angleLeft = Mathf.Atan2(left.y, left.x) * Mathf.Rad2Deg;
        float angleRight = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;
        if (angleLeft > 90 || angleLeft <0) //these need to be tweaked
            return -1;
        if (angleRight > 90 || angleRight < 0)
            return 1;
        return 0;
    }

    private float CalculateMagnitude(Vector2 vector)
    {
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
    }

    private void turnLeftRight()
    {
        if (direcitonInput == -1 && !facingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingLeft = true;
        }
        else if (direcitonInput == 1 && facingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingLeft = false;
        }
    }

    private void calculateJump()
    {
        if (!isGrounded)
            jumpExtraction = rb.velocity - gravityForce - moveDirection;
        else
            jumpExtraction = Vector2.zero;

        jumpMagnitude = CalculateMagnitude(jumpExtraction);


        // Jump if grounded and space is pressed
        rotatedX = -gravityDirection.x;
        rotatedY = -gravityDirection.y;
        if (space && isGrounded)
            jump = new Vector2(rotatedX * jumpForce, rotatedY * jumpForce);
        else
            jump = new Vector2(0, 0);
    }

    private void calculateJetPackHover() //might change this to space man only
    {
        rotatedX = -gravityDirection.x;
        rotatedY = -gravityDirection.y;
        if (space && groundTimer.checkTimer() && currentFuel > 0)
        {
            hoverFlag = true;
            useFuel();
        }

        if (!space || currentFuel == 0)
            hoverFlag = false;

        hover = hoverFlag ? new Vector2(rotatedX * jetPackForce, rotatedY * jetPackForce) : Vector2.zero;
    }

    private void useFuel()
    {
        float fuelConsumptionRate = 100f; // Fuel units per second
          currentFuel -= fuelConsumptionRate * Time.deltaTime; // Decrease fuel over time

        if (currentFuel < 0)
            currentFuel = 0; // Prevent negative fuel
    }

    private void calculateMovement()
    {
        //movement
        rotatedX = -gravityDirection.y;
        rotatedY = gravityDirection.x;
        if (wallInFront() == (int)horizontalInput)
            horizontalInput = 0;
        moveDirection = new Vector2((horizontalInput * moveSpeed * rotatedX), (horizontalInput * moveSpeed * rotatedY));
    }

    private void calculateDrag()
    {
        //drag calculation
        if (isGrounded)
        {
            drag = previousV;
        }
        else
            drag = previousMove;

    }

    private void determineAnimation()
    {
        try
        {
            if (horizontalInput == 0 /*|| !isGrounded uncomment when is grounded is more robust*/) //for idle
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Backwards", false);
            }
            else if (jump != Vector2.zero)
            {
                animator.SetBool("Jump", true);
            }
            else //for walking
            {
                if ((facingLeft && horizontalInput == 1) || (!facingLeft && horizontalInput == -1))
                {
                    animator.SetBool("Walk", true);
                    animator.SetBool("Backwards", true);
                }
                else
                {
                    animator.SetBool("Walk", true);
                    animator.SetBool("Backwards", false);
                }
            }
        }
        catch (Exception e)
        {
            bool ham = false;
        }
    }

    public void setMovement(int moveInput)
    {
        horizontalInput = moveInput;
    }

    public void setOrientation(int direction)
    {
        direcitonInput = direction;
    }

    public void setJump(bool jumpInput)
    {
        space = jumpInput;
    }

    public void setHealth(float newHealth)
    {
        health = newHealth;
    }

    public void setMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }

    public void setThrow(bool throwInput)
    {
        throwItem = throwInput;
    }

    public void setClick(bool clickInput)
    {
        click = clickInput;
    }

    public Collider2D getCharacterCollider()
    {
        return collider;
    }

    public Vector2 getGravityDirection()
    {
        return gravityDirection;
    }

    public float getCharacterOrientation()
    {
        return transform.rotation.eulerAngles.z;
    }

    public bool getFacingLeft()
    {
        return facingLeft;
    }

    public float getHealth()
    {
        return health;
    }

    public bool getThrow()
    {
        return throwItem;
    }

    public bool getClick()
    {
        return click;
    }
}
