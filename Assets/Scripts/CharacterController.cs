using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : ObjectController{
    //object creation
    CircleCollider2D circleColliderPlayer;

    //constants
    public float moveSpeed = 20f;
    public float jumpForce = 11f;

    //game variables
    float heightTestPlayer = 0;
    float horizontalInput = 0;
    float direcitonInput = 0;
    float rotatedX = 0;
    float rotatedY = 0;
    float jumpMagnitude = 0;
    float maxHealth = 3f; //default max health is 3
    float health = 0f;
    bool isGrounded = false;
    bool space = false;
    bool facingLeft = false;
    

    //vectors
    Vector2 moveDirection = new Vector2(0, 0);
    Vector2 jump = new Vector2(0, 0);
    Vector2 previousV = new Vector2(0, 0);
    Vector2 drag = new Vector2(0, 0);
    Vector2 previousMove = new Vector2(0, 0);
    Vector2 jumpExtraction = new Vector2(0, 0);
    Vector2 additionalForce = new Vector2(0, 0);

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

    public CircleCollider2D getCharacterCollider()
    {
        return circleColliderPlayer;
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


    public void calculateCharacterStart()
    {
        health = maxHealth;
        calculateStart();      
        circleColliderPlayer = GetComponent<CircleCollider2D>();
        heightTestPlayer = circleColliderPlayer.bounds.extents.y + 0.05f;

    }

    public void calculateCharacterUpdate()
    {
        turnLeftRight(); 

        //Check if the player is grounded
        isGrounded = IsGrounded();


        calculateJump();
        calculateMovement();
        calculateDrag();

        rb.AddForce(jump, ForceMode2D.Impulse);

        if (!wallInFront())
        {
            rb.AddForce(moveDirection, ForceMode2D.Impulse);
            rb.AddForce(drag, ForceMode2D.Impulse); //drag is needed because negate the old velcotiy so you can account for hte new agnel and recalculate

            //what this line does is if the player is in the air, it automatically adjusts its jump arc to follow gravity
            if (!isGrounded)
                rb.velocity += -jumpExtraction + jumpMagnitude * -gravityDirection; 
        }
        calculateUpdate();
        

        previousV = -rb.velocity;
        previousMove = -moveDirection;
    }

    public void addForceLocal(Vector2 force)
    {
        //Vector2 localForce = new Vector2(0, 0);

        float angle = Vector2.SignedAngle(gravityDirection, force);
        additionalForce = force;
       // Debug.Log(localForce);
        //rb.AddForce(localForce);
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(circleColliderPlayer.bounds.center, gravityDirection, heightTestPlayer, layerMaskPlanet);
        bool ground = hit.collider != null;
        return ground;
    }

    bool wallInFront()
    {
        int layerMask = LayerMask.GetMask("Default", "enemy", "player");
        int currentLayer = gameObject.layer;
        int finalMask = layerMask & ~(1 << currentLayer);

        RaycastHit2D hit = Physics2D.Raycast(circleColliderPlayer.bounds.center, facingLeft ? -Vector2.Perpendicular(gravityDirection) : Vector2.Perpendicular(gravityDirection), heightTestPlayer + 3, finalMask);
        Vector2 groundNormal = hit.normal; // The normal of the surface
        float angle = Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle <0)
            return true;
        return false;
    }

    float CalculateMagnitude(Vector2 vector)
    {
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
    }

    void turnLeftRight()
    {
        if (direcitonInput == -1 && facingLeft == false)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingLeft = true;
        }
        else if (direcitonInput == 1 && facingLeft == true)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingLeft = false;
        }
    }

    void calculateJump()
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

    void calculateMovement()
    {
        //movement
        rotatedX = -gravityDirection.y;
        rotatedY = gravityDirection.x;
        moveDirection = new Vector2((horizontalInput * moveSpeed * rotatedX), (horizontalInput * moveSpeed * rotatedY));
    }

    void calculateDrag()
    {
        //drag calculation
        if (isGrounded)
        {
            drag = previousV;
        }
        else
            drag = previousMove;

    }


}
