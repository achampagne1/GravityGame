using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

public enum CHARACTERSTATE
{
    IDLE,
    WALKING,
    AIMING,
    AIRBORNE, //AIRBORNING?
    GROUNDED,
    HURT,
    RELAXED,
}

public enum CHARACTERSUBSTATE
{
    NONE,
    FORWARD,
    BACKWARD,
    UP,
    DOWN,
}

public struct CharacterStateNode
{
    public CHARACTERSTATE characterState;
    public CHARACTERSUBSTATE subState; //used for things like airborne up/down, walking left/right

    public CharacterStateNode(CharacterStateNode other)
    {
        this.characterState = other.characterState;
        this.subState = other.subState;
    }
}

public class CharacterController : ObjectController,IHealth
{
    //object creation
    protected Animator animator;
    [SerializeField] protected Sprite hitSprite;
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected GameObject explodeTemplate;
    [SerializeField] private GameObject hitMark;

    //public game variables
    [SerializeField] protected bool invincibleFlag = false;
    [SerializeField] protected bool shieldUpFlag = false;
    [SerializeField] protected bool forceLocalAdded = false;
    [SerializeField] protected bool movementToggle = true;
    [SerializeField] protected float jumpForce = 11f;
    [SerializeField] protected float moveSpeed = 20f;
    [SerializeField] protected float maxHealth = 3f; //default max health is 3
    [SerializeField] protected float health = 0f;
    [SerializeField] private float knockBackDuration = .1f;
    [SerializeField] protected float groundedGracePeriod = 0.1f;
    [SerializeField] private Vector2 hitMarkCenter = Vector2.zero;

    //protected game variables
    protected bool click = false;
    protected bool space = false;
    protected bool dead = false;
    protected bool strikeLeftLatch = false;
    protected bool hitLatch = false;
    protected int wallInFrontVar = 0;
    protected float rotatedX = 0;
    protected float rotatedY = 0;
    protected float direcitonInput = 1;

    //private game variables
    private float jumpMagnitude = 0;
    private float horizontalInput = 0;
    private List<CharacterStateNode> characterStates = new List<CharacterStateNode>();
    private Coroutine hurtStateCoroutine;
    private bool hurtStateActive;

    //vectors
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 jump = Vector2.zero;
    private Vector2 previousV = Vector2.zero;
    private Vector2 drag = Vector2.zero;
    private Vector2 previousMove = Vector2.zero;
    private Vector2 jumpExtraction = Vector2.zero;
    private Vector2 additionalForce = Vector2.zero;
    protected Vector2 bulletStrikeLocation;
    protected Vector2 lookingDirection = Vector2.zero;

    protected List<CharacterProp> characterProps;



    // Start is called before the first frame update
    public override void Start()
    {

        health = maxHealth;
        characterProps = new List<CharacterProp>();

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

        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<CharacterProp>() != null)
                characterProps.Add(child.gameObject.GetComponent<CharacterProp>());
        }

        base.Start();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        characterStates.Clear();
        resetAnimationVars();

        turnLeftRight();
        wallInFrontVar = wallInFront();
        calculateJump();
        calculateMovement();
        calculateDrag();
        detectLedge();

        //NOTE: when a force local is added, movement,jump, and drag are ignored. mioght redo in the future
        if (!forceLocalAdded)
        {
            rb.AddForce(jump, ForceMode2D.Impulse);
            rb.AddForce(moveDirection, ForceMode2D.Impulse);
            rb.AddForce(drag, ForceMode2D.Impulse); //drag is needed because negate the old velcotiy so you can account for hte new agnel and recalculate
            rb.linearVelocity += -jumpExtraction + jumpMagnitude * -gravityDirection; //what this line does is if the player is in the air, it automatically adjusts its jump arc to follow gravit
        }

        characterStateMachine();
        foreach (CharacterProp prop in characterProps)
        {
            prop.setFacingLeft(facingLeft);
            prop.setInputDirection(lookingDirection);
            prop.setCharacterStates(characterStates);
        }

        base.FixedUpdate();
        
        previousV = -rb.linearVelocity;
        previousMove = -moveDirection;

        if (health < 0)
            health = 0;

        if (health == 0 && !dead)
        {
            StartCoroutine(die());
            dead = true;
        }

        strikeLeftLatch = false;
        hitLatch = false;
    }

    private void resetAnimationVars()
    {
        foreach(AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name,false);
        }
    }

    public override void triggerLogic(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Projectile" || trigger.gameObject.tag == "Hazard" || trigger.gameObject.tag == "Melee")
            hit(trigger.gameObject);

        base.triggerLogic(trigger);
    }

    public virtual void hit(GameObject hitGameObject)
    {
        hitLatch = true;
        if (shieldUpFlag && hitGameObject.tag=="Projectile") //should be dependency injection?
            return;

        if(!hitGameObject.GetComponent<IDamager>().damage(gameObject))
            return;

        spawnHitMark();

        if (hurtStateCoroutine != null)
            StopCoroutine(hurtStateCoroutine);
        hurtStateCoroutine = StartCoroutine(hurtStateForDuration());

        SoundManager.instance.playSound(hitSound, transform, 1f);
        StartCoroutine(HitHelper.changeColorWrapper(gameObject));


        if (hitGameObject.tag == "Hazard" || hitGameObject.tag == "Melee") //dependency injection?
            return;
        
        var (strikeLocation, rotation) = StrikeLocation.determineStrikeLocation(hitGameObject, gameObject, heightObject);

        bulletStrikeLocation = strikeLocation;
        strikeLeftLatch = HitHelper.leftStrikeLocation(gameObject, strikeLocation, facingLeft);
        StartCoroutine(knockBack(strikeLeftLatch));

        IEnumerator knockBack(bool strikeLeft) //id like to move this to hit helper but you cant pass in ref for coroutines
        {
            forceLocalAdded = true; //need to pass in a reference to the gameobjects force lcoal added
            forceLocal = Quaternion.Euler(0f, 0f, gameObject.transform.eulerAngles.z) * new Vector2(strikeLeft ? 3f : -3f, 3f);
            yield return new WaitForSeconds(knockBackDuration);
            forceLocalAdded = false;
            yield return null;
        }
    }

    private void spawnHitMark()
    {
        Vector3 center = transform.TransformPoint(new Vector3(hitMarkCenter.x, hitMarkCenter.y, 0f));
        Vector2 offset = UnityEngine.Random.insideUnitCircle *.1f;
        Vector3 position = center + transform.TransformDirection(new Vector3(offset.x, offset.y, 0f));
        Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
        GameObject mark = Instantiate(hitMark, position, rotation);
        mark.transform.localScale *= UnityEngine.Random.Range(0.9f, 1.1f);
        SpriteRenderer hitMarkSR = mark.GetComponent<SpriteRenderer>();
        hitMarkSR.sortingOrder = spriteRenderer.sortingOrder +1;
        Destroy(mark, 0.1f);
    }

    protected virtual IEnumerator die()
    {
        if(explodeTemplate != null)
        {
            GameObject explodeClone = Instantiate(explodeTemplate, transform.position, Quaternion.identity);
            ExplodeController explodeController = explodeClone.GetComponent<ExplodeController>();
            explodeController.trigger(bulletStrikeLocation);
        }

        Destroy(gameObject);
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
        moveDirection.x = (horizontalInput * moveSpeed * rotatedX);
        moveDirection.y = (horizontalInput * moveSpeed * rotatedY);
    }

    private void calculateJump()
    {
        if (!isGrounded)
            jumpExtraction = rb.linearVelocity - gravityForce - moveDirection;
        else
            jumpExtraction = Vector2.zero;

        jumpMagnitude = CalculateMagnitude(jumpExtraction);


        // Jump if grounded and space is pressed
        rotatedX = -gravityDirection.x;
        rotatedY = -gravityDirection.y;
        if (space && isGrounded)
        {
            jump.x = rotatedX * jumpForce;
            jump.y = rotatedY * jumpForce;
        }
        else
            jump = Vector2.zero;
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
            //NOTE: doing this is more Garbage OCllector friendly
            Vector3 scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
            facingLeft = true;
        }
        else if (direcitonInput == 1 && facingLeft)
        {
            Vector3 scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
            facingLeft = false;
        }
        facingLeftInt = facingLeft ? -1 : 1;
    }

    protected virtual void characterStateMachine()
    {
        if (hurtStateActive)
        {
            addCharacterState(CHARACTERSTATE.HURT);
        }

        if (isGrounded || groundStopWatch.getElapsedTime() < groundedGracePeriod)
        {
            addCharacterState(CHARACTERSTATE.GROUNDED);
            if (horizontalInput != 0)
            {
                CHARACTERSUBSTATE subState;
                if((facingLeft && horizontalInput == 1) || (!facingLeft && horizontalInput == -1))
                {
                    subState = CHARACTERSUBSTATE.BACKWARD;
                }
                else
                {
                    subState = CHARACTERSUBSTATE.FORWARD;
                }
                addCharacterState(CHARACTERSTATE.WALKING,subState);
            }
            else
            {
                addCharacterState(CHARACTERSTATE.IDLE);
            }
        }
        else
        {
            CHARACTERSUBSTATE subState;
            if (up)
            {
                subState = CHARACTERSUBSTATE.UP;
            }
            else
            {
                subState = CHARACTERSUBSTATE.DOWN;
            }
            addCharacterState(CHARACTERSTATE.AIRBORNE,subState);
        }
    }

    private IEnumerator hurtStateForDuration()
    {
        hurtStateActive = true;
        yield return new WaitForSeconds(0.1f);
        hurtStateActive = false;
        hurtStateCoroutine = null;
    }

    protected void addCharacterState(CHARACTERSTATE characterState, CHARACTERSUBSTATE subState = CHARACTERSUBSTATE.NONE)
    {
        CharacterStateNode characterStateNode = new CharacterStateNode()
        {
            characterState = characterState,
            subState = subState
        };

        characterStates.Add(characterStateNode);

        if (animator == null)
            return;

        try
        {
            animator.SetBool(characterState.ToString(), true);
            if (subState != CHARACTERSUBSTATE.NONE)
            {
                animator.SetBool(subState.ToString(), true);
            }
            return;
        }
        catch
        {
            return;
        }
    }

    public void setMovement(int moveInput)
    {
        horizontalInput = moveInput;
    }

    public void setJump(bool jump)
    {
        space = jump;
    }

    public void setOrientation(int direction)
    {
        direcitonInput = direction;
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

    public void setInvincible(bool invincibleFlag)
    {
        this.invincibleFlag = invincibleFlag;
    }

    public void setShieldUp(bool shieldUpFlag)
    {
        this.shieldUpFlag = shieldUpFlag;
    }

    public void setForceLocalAdded(bool forceLocalAdded)
    {
        this.forceLocalAdded = forceLocalAdded;
    }

    public Vector2 getGravityDirection()
    {
        return gravityDirection;
    }

    public float getCharacterOrientation()
    {
        return transform.rotation.eulerAngles.z;
    }

    public float getHealth()
    {
        return health;
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public bool getDead()
    {
        return dead;
    }
}
