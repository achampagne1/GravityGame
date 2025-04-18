using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class CharacterController : ObjectController
{
    //object creation
    private Animator animator;
    protected AudioController audioController;
    protected ExplodeController explodeController;
    [SerializeField] protected GameObject explodeCenter;

    //public game variables
    [SerializeField] protected float jumpForce = 11f;
    [SerializeField] protected float moveSpeed = 20f;
    [SerializeField] protected float maxHealth = 3f; //default max health is 3
    [SerializeField] protected  bool invincibleFlag = false;
    [SerializeField] protected float health = 0f;
    protected float rotatedX = 0;
    protected float rotatedY = 0;
    protected bool click = false;

    //private game variables
    private float jumpMagnitude = 0;
    private float horizontalInput = 0;
    private float direcitonInput = 0;
    protected int wallInFrontVar = 0;
    protected bool space = false;
    private bool facingLeft = false;
    protected bool dead = false;

    //vectors
    private Vector2 moveDirection = new Vector2(0, 0);
    private Vector2 jump = new Vector2(0, 0);
    private Vector2 previousV = new Vector2(0, 0);
    private Vector2 drag = new Vector2(0, 0);
    private Vector2 previousMove = new Vector2(0, 0);
    private Vector2 jumpExtraction = new Vector2(0, 0);
    private Vector2 additionalForce = new Vector2(0, 0);
    private Vector3 bulletStrikeLocation = new Vector3(0, 0, 0);



    // Start is called before the first frame update
    public void calculateCharacterStart()
    {
        Physics2D.IgnoreLayerCollision(9, 9, true);
        Physics2D.IgnoreLayerCollision(9, 11, true);
        Physics2D.IgnoreLayerCollision(11, 11, true);

        audioController = GetComponent<AudioController>();

        health = maxHealth;

        foreach (Transform child in transform) //TODO: fiund a way to do only one loop for the spaceperson
        {
            if (child.name == "Explode")
                explodeController = child.gameObject.GetComponent<ExplodeController>();
        }

        try
        {
            animator = GetComponent<Animator>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        if (transform.localScale.x < 0)
            facingLeft = true;

        calculateStart();
    }

    // Update is called once per frame
    public void calculateCharacterUpdate()
    {
        turnLeftRight();
        determineAnimation();
        wallInFrontVar = wallInFront();
        calculateJump();
        calculateMovement();
        calculateDrag();
        detectLedge();

        rb.AddForce(jump, ForceMode2D.Impulse);
        rb.AddForce(moveDirection, ForceMode2D.Impulse);
        rb.AddForce(drag, ForceMode2D.Impulse); //drag is needed because negate the old velcotiy so you can account for hte new agnel and recalculate

        //what this line does is if the player is in the air, it automatically adjusts its jump arc to follow gravity
        if (!isGrounded)
            rb.velocity += -jumpExtraction + jumpMagnitude * -gravityDirection;

        calculateUpdate();

        previousV = -rb.velocity;
        previousMove = -moveDirection;

        if (health == 0 && !dead)
        {
            StartCoroutine(die());
            dead = true;
        }

    }

    protected virtual IEnumerator die()
    {
        try
        {
            explodeCenter.transform.position = bulletStrikeLocation;
        }
        catch
        {
            Debug.LogError("no center found");
        }

        if(explodeController!=null)
            explodeController.trigger();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.0f;
        sr.color = c;
        yield return null;
    }

        public void addForceLocal(Vector2 force)
    {
        float angle = Vector2.SignedAngle(gravityDirection, force);
        additionalForce = force;
    }

    private float CalculateMagnitude(Vector2 vector)
    {
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
    }

    protected bool detectLedge()
    {
        Vector2 localDirection = new Vector2(1, -1).normalized;
        if (facingLeft)
            localDirection.x = localDirection.x * -1;

        int mask = (1 << 15 | 1 << 0);
        Vector2 worldDirection = transform.TransformDirection(localDirection);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, worldDirection, heightObject + 1f, mask);
        if (hit.collider == null)
            return true;
        return false;
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

    private void calculateMovement()
    {
        //movement
        rotatedX = -gravityDirection.y;
        rotatedY = gravityDirection.x;
        if (wallInFrontVar == (int)horizontalInput)
            horizontalInput = 0;
        moveDirection = new Vector2((horizontalInput * moveSpeed * rotatedX), (horizontalInput * moveSpeed * rotatedY));
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

    private int wallInFront()
    {
        int layerMask = LayerMask.GetMask("Default", "Platforms"); //how can this be done better
        int currentLayer = gameObject.layer;
        int finalMask = layerMask & ~(1 << currentLayer);

        Vector2 left = Physics2D.Raycast(transform.position, -Vector2.Perpendicular(gravityDirection), heightObject + .2f, finalMask).normal;
        Vector2 right = Physics2D.Raycast(transform.position, Vector2.Perpendicular(gravityDirection), heightObject + .2f, finalMask).normal;
        float angleLeft = Mathf.Atan2(left.y, left.x) * Mathf.Rad2Deg;
        float angleRight = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;
        if (angleLeft > 90 || angleLeft < 0) //these need to be tweaked
            return -1;
        if (angleRight > 90 || angleRight < 0)
            return 1;
        return 0;
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

    private void determineAnimation() //might need to be redone to allow for differences between space eprson and chgaracter
    {
        try
        {
            if (isGrounded)
            {
                animator.SetBool("Airborn", false);
                if (horizontalInput == 0)
                {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Backwards", false);
                }
                else
                {
                    animator.SetBool("Walk", true);
                    if ((facingLeft && horizontalInput == 1) || (!facingLeft && horizontalInput == -1))
                        animator.SetBool("Backwards", true);
                    else
                        animator.SetBool("Backwards", false);
                }
            }
            else
            {
                animator.SetBool("Airborn", true);
                if (up)
                    animator.SetBool("Up", true);
                else
                    animator.SetBool("Up", false);
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
        if (!invincibleFlag)
            health = newHealth;
    }

    public void setMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }
    public void setClick(bool clickInput)
    {
        click = clickInput;
    }

    public Collider2D getCharacterCollider()
    {
        return GetComponent<Collider2D>();
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

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public void setBulletStrikeLocation(Vector3 bulletStrikeLocation)
    {
        this.bulletStrikeLocation = bulletStrikeLocation;
    }

}
