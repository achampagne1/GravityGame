using System.Collections;
using UnityEngine;

public class GunParentedEffect : MonoBehaviour, IParentedEffect
{
    [SerializeField] private float recoilMultiplier = 100f;
    [SerializeField] private float recoilSpeedMultiplier = 1.0f;
    [SerializeField] private float recoilReturnSpeed = .2f;

    public float float1 { get; set; }
    public int facingLeftInt { get; set; }
    public Vector3 inputDirection { get; set; }
    public Vector3 originalPosition { get; set; }
    public Vector2 maxRotationalAngle { get; set; }
    public Transform prop { get; set; }

    private Coroutine recoilCoroutine;

    public void parentedEffect(Transform prop,int facingLeftInt, Vector3 inputDirection,Vector3 originalPosition,Vector2 maxRotationalAngle)
    {
        this.prop = prop;
        this.facingLeftInt = facingLeftInt;
        this.inputDirection = inputDirection;
        this.originalPosition = originalPosition;
        recoil(float1);
    }

    protected void recoil(float magnitude)
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
            rotateAboutCenter(inputDirection, originalPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        inputDirection = previousDirection;
        rotateAboutCenter(inputDirection, originalPosition);
        recoilCoroutine = null;
    }
    protected void rotateAboutCenter(Vector2 direction, Vector3 position)
    {
        Vector2 localLookingDirection = prop.parent.InverseTransformDirection(direction * facingLeftInt);
        float angle = Mathf.Atan2(localLookingDirection.y, localLookingDirection.x) * Mathf.Rad2Deg;
        float finalAngle;

        if (facingLeftInt == 1)
        {
            finalAngle = Mathf.Clamp(angle, maxRotationalAngle.y, maxRotationalAngle.x);
        }
        else
        {
            finalAngle = Mathf.Clamp(angle, -maxRotationalAngle.x, -maxRotationalAngle.y);
        }

        Quaternion lookRotation = Quaternion.Euler(0f, 0f, finalAngle * facingLeftInt);
        prop.localPosition = lookRotation * position;
        prop.localRotation = lookRotation;
    }

}
