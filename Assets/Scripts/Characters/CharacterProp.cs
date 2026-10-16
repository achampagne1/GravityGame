using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class CharacterProp : MonoBehaviour, ILeft
{
    public bool facingLeft { get; set; } = false;
    public int facingLeftInt { get; set; } = 1;

    [SerializeField] private Vector2 maxRotationalAngle = new Vector2(-180, 180);
    [SerializeField] private float transitionSpeed = 20f;
    protected IItemEffect itemParentedEffect = null;
    protected Quaternion originalRotation;
    protected Vector3 originalPosition;
    protected Vector3 originalScale;
    protected Vector3 inputDirection = Vector3.zero;
    protected Vector3 inputDirectionLatch = Vector3.zero;
    [SerializeField] protected float inputLatchSensitivity = .1f;
    [SerializeField] protected Vector2 animationOffset = Vector2.zero;
    [SerializeField] protected Vector3 rotationOffset = Vector3.zero;
    private float transitionInterpolator = 0.0f;
    private Quaternion appliedRotationOffset = Quaternion.identity;
    private Animator animator;

    protected List<CharacterStateNode> characterStates = new List<CharacterStateNode>();
    protected List<CharacterStateNode> stateLatch = new List<CharacterStateNode>();

    public virtual void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
        stateLatch = characterStates;

        animator = GetComponent<Animator>();
    }

    public virtual void FixedUpdate()
    {
        transform.localRotation *= Quaternion.Inverse(appliedRotationOffset);

        if(!characterStates.SequenceEqual(stateLatch))
        {
            transitionInterpolator = 0.0f;
        }

        characterStateMachine();
        
        transform.localPosition += (Vector3)animationOffset;
        appliedRotationOffset = Quaternion.Euler(rotationOffset);
        transform.localRotation *= appliedRotationOffset;
        stateLatch = characterStates;
        transitionInterpolator = transitionSpeed * Time.deltaTime;
        Mathf.Clamp(transitionInterpolator, 0.0f, 1.0f);
    }

    protected virtual void characterStateMachine()
    {
        foreach (CharacterStateNode state in characterStates)
        {
            switch (state.characterState)
            {
                case CHARACTERSTATE.AIMING:
                    aimingState();
                    break;
                case CHARACTERSTATE.WALKING:
                    walkingState();
                    break;
                case CHARACTERSTATE.RELAXED:
                default:
                    relaxedState();
                    break;
            }
        }
    }

    protected virtual void relaxedState()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, transitionInterpolator);
        Vector3 offset = animationOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition - offset, originalPosition, transitionInterpolator);
        transform.localScale = originalScale;
        safeAnimationSetState("Idle", true);
    }

    protected virtual void walkingState()
    {
        relaxedState();
        safeAnimationSetState("Walk", true); //overrides the previously set idle stat ewith the walking state
    }

    protected virtual void aimingState()
    {
        HelperFunctions.rotateAboutPoint(transform, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle);
        safeAnimationSetState("Aim", true);
    }

    public void useEffect()
    {
        if(itemParentedEffect != null)
            itemParentedEffect.effect(inputDirection,originalPosition,maxRotationalAngle);
    }

    public void setCharacterStates(List<CharacterStateNode> characterStates)
    {
        this.characterStates = characterStates;
    }

    public void setInputDirection(Vector3 inputDirection)
    {
        this.inputDirection = inputDirection;
    }

    public void setFacingLeft(bool facingLeft)
    {
        this.facingLeft = facingLeft;
        facingLeftInt = facingLeft ? -1 : 1;
    }

    public void setItemParentedEffect(IItemEffect itemEffect)
    {
        itemParentedEffect = itemEffect;
    }   

    private bool safeAnimationSetState(string parameterName, bool value)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }

        try
        {
            animator.SetBool(parameterName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected bool containsState(List<CharacterStateNode> states, CHARACTERSTATE state) { 
        foreach(CharacterStateNode node in states)
        {
            if (node.characterState == state)
                return true;
        }
        return false;
    }
}
