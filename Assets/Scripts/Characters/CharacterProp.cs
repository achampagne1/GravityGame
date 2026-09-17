using UnityEngine;

public class CharacterProp : MonoBehaviour
{
    [SerializeField] private Vector2 maxRotationalAngle = new Vector2(-180, 180);
    [SerializeField] private float transitionSpeed = 20f;
    protected Quaternion originalRotation;
    protected Vector3 originalPosition;
    protected Vector3 originalScale;
    protected Vector3 inputDirection = Vector3.zero;
    protected bool facingLeft = false;
    private int facingLeftInt = 1;
    private float transitionInterpolator = 0.0f;

    [SerializeField] protected CHARACTERSTATE characterState = CHARACTERSTATE.IDLE;
    protected CHARACTERSTATE stateLatch;

    public virtual void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
        stateLatch = characterState;
    }

    public virtual void FixedUpdate()
    {
        if(stateLatch != characterState)
        {
            transitionInterpolator = 0.0f;
        }

        switch (characterState)
        {
            case CHARACTERSTATE.AIMING:
                aimingState();
                break;
            case CHARACTERSTATE.IDLE:
            default:
                idleState();
                break;
        }
        stateLatch = characterState;
        transitionInterpolator = transitionSpeed * Time.deltaTime;
        Mathf.Clamp(transitionInterpolator, 0.0f, 1.0f);
    }

    protected virtual void idleState()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, transitionInterpolator);
        transform.localPosition = Vector3.Lerp(transform.localPosition,originalPosition, transitionInterpolator);
        transform.localScale = originalScale;
    }

    protected virtual void aimingState()
    {
        Vector2 localLookingDirection = transform.parent.InverseTransformDirection(inputDirection * facingLeftInt);
        float angle = Mathf.Atan2(localLookingDirection.y, localLookingDirection.x) * Mathf.Rad2Deg;
        float finalAngle;

        if (!facingLeft)
        {
            finalAngle = Mathf.Clamp(angle, maxRotationalAngle.y, maxRotationalAngle.x);
        }
        else
        {
            finalAngle = Mathf.Clamp(angle, -maxRotationalAngle.x, -maxRotationalAngle.y);
        }

        Quaternion lookRotation = Quaternion.Euler(0f, 0f, finalAngle * facingLeftInt);
        transform.localPosition = lookRotation * originalPosition;
        transform.localRotation = lookRotation;
    }

    public void setCharacterState(CHARACTERSTATE newState)
    {
        characterState = newState;
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
}
