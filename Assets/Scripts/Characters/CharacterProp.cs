using UnityEngine;
using System.Collections;

public class CharacterProp : MonoBehaviour
{
    [SerializeField] private Vector2 maxRotationalAngle = new Vector2(-180, 180);
    [SerializeField] private float transitionSpeed = 20f;
    [SerializeField] private float recoilMultiplier = 100f;
    [SerializeField] private float recoilSpeedMultiplier = 1.0f;
    [SerializeField] private float recoilReturnSpeed = .2f;
    protected IParentedEffect itemParentedEffect = null;
    protected Quaternion originalRotation;
    protected Vector3 originalPosition;
    protected Vector3 originalScale;
    protected Vector3 inputDirection = Vector3.zero;
    protected bool facingLeft = false;
    protected int facingLeftInt = 1;
    private float transitionInterpolator = 0.0f;
    protected Coroutine recoilCoroutine;

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
        rotateAboutCenter(inputDirection,originalPosition);
    }

    protected void rotateAboutCenter(Vector2 direction,Vector3 position)
    {
        Vector2 localLookingDirection = transform.parent.InverseTransformDirection(direction * facingLeftInt);
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
        transform.localPosition = lookRotation * position;
        transform.localRotation = lookRotation;
    }

    protected void useParentedEffectRequested()
    {
        if(itemParentedEffect != null)
            itemParentedEffect.parentedEffect();
    }

    protected void recoil(Vector2 direction, float magnitude)
    {
        //originalPosition = new Vector3(armLengthActive, 0, 0);
        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        Vector3 previousDirection = inputDirection;
        float inputAngle = Mathf.Atan2(previousDirection.y, previousDirection.x) * Mathf.Rad2Deg;
        float recoilAngle = inputAngle + magnitude * recoilMultiplier;
        Vector3 recoilDirection = Quaternion.Euler(0f, 0f, recoilAngle) * Vector3.right;
        recoilCoroutine = StartCoroutine(recoilRoutine(previousDirection, recoilDirection, magnitude));
    }

    protected IEnumerator recoilRoutine(Vector3 previousDirection, Vector3 recoilDirection, float magnitude)
    {
        float elapsedTime = 0f;
        float kickDuration = recoilSpeedMultiplier * magnitude;
        recoilDirection.y = recoilDirection.y * facingLeftInt;

        while (elapsedTime < kickDuration)
        {
            inputDirection = Vector3.Lerp(previousDirection, recoilDirection, elapsedTime / kickDuration);
            rotateAboutCenter(inputDirection, originalPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < recoilReturnSpeed)
        {
            inputDirection = Vector3.Lerp(recoilDirection, previousDirection, elapsedTime / recoilReturnSpeed);
            rotateAboutCenter(inputDirection,originalPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        inputDirection = previousDirection;
        rotateAboutCenter(inputDirection, originalPosition);
        recoilCoroutine = null;
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
