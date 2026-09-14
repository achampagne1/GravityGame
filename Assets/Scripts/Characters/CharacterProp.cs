using UnityEngine;

public class CharacterProp : MonoBehaviour
{
    [SerializeField] private Vector2 maxRotationalAngle = new Vector2(-180, 180);
    protected Quaternion originalRotation;
    protected Vector3 originalPosition;
    protected Vector3 originalScale;
    protected Vector3 inputDirection = Vector3.zero;
    private bool facingLeft = false;
    private int facingLeftInt = 1;

    [SerializeField] private CHARACTERSTATE characterState = CHARACTERSTATE.IDLE;

    public virtual void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
    }

    public virtual void FixedUpdate()
    {
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
    }

    protected virtual void idleState()
    {
        transform.localRotation = originalRotation;
        transform.localPosition = originalPosition;
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
