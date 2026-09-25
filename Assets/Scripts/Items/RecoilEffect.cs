using System.Collections;
using UnityEngine;

public class RecoilEffect : MonoBehaviour, IItemEffect
{
    [SerializeField] private float recoilMultiplier = 100f;
    [SerializeField] private float recoilSpeedMultiplier = 1.0f;
    [SerializeField] private float recoilReturnSpeed = .2f;

    public float float1 { get; set; }
    public int facingLeftInt { get; set; }
    public Vector3 inputDirection { get; set; }
    public Vector3 originalPosition { get; set; }
    public Vector2 maxRotationalAngle { get; set; }
    public Vector3 point { get; set; }

    private Coroutine recoilCoroutine;
    private float recoilOffsetAngle;

    public void effect(int facingLeftInt, Vector3 inputDirection, Vector3 originalPosition, Vector2 maxRotationalAngle, Vector3 point = default(Vector3))
    {
        this.facingLeftInt = facingLeftInt;
        this.inputDirection = inputDirection;
        this.originalPosition = originalPosition;
        this.maxRotationalAngle = maxRotationalAngle;
        this.point = point;
        recoil(inputDirection, float1);
    }

    protected void recoil(Vector3 baseDirection, float magnitude)
    {
        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        float startingOffset = recoilOffsetAngle;
        float targetOffset = startingOffset + magnitude * recoilMultiplier * facingLeftInt;
        recoilCoroutine = StartCoroutine(recoilRoutine(baseDirection, startingOffset, targetOffset, magnitude));
    }

    protected IEnumerator recoilRoutine(Vector3 baseDirection, float startingOffset, float targetOffset, float magnitude)
    {
        float elapsedTime = 0f;
        float kickDuration = recoilSpeedMultiplier * magnitude;

        while (elapsedTime < kickDuration)
        {
            recoilOffsetAngle = Mathf.Lerp(startingOffset, targetOffset, elapsedTime / kickDuration);
            inputDirection = Quaternion.Euler(0f, 0f, recoilOffsetAngle) * baseDirection;
            HelperFunctions.rotateAboutPoint(transform, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle, point);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        recoilOffsetAngle = targetOffset;
        elapsedTime = 0f;
        while (elapsedTime < recoilReturnSpeed)
        {
            recoilOffsetAngle = Mathf.Lerp(targetOffset, 0f, elapsedTime / recoilReturnSpeed);
            inputDirection = Quaternion.Euler(0f, 0f, recoilOffsetAngle) * baseDirection;
            HelperFunctions.rotateAboutPoint(transform, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle, point);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        recoilOffsetAngle = 0f;
        inputDirection = baseDirection;
        HelperFunctions.rotateAboutPoint(transform, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle, point);
        recoilCoroutine = null;
    }

    public void copyData(IItemEffect otherEffect)
    {
        RecoilEffect gunParentedEffect = otherEffect as RecoilEffect;
        recoilMultiplier = gunParentedEffect.recoilMultiplier;
        recoilSpeedMultiplier = gunParentedEffect.recoilSpeedMultiplier;
        recoilReturnSpeed = gunParentedEffect.recoilReturnSpeed;
        float1 = gunParentedEffect.float1;
        facingLeftInt = gunParentedEffect.facingLeftInt;
        inputDirection = new Vector3(gunParentedEffect.inputDirection.x, gunParentedEffect.inputDirection.y, gunParentedEffect.inputDirection.z);
        originalPosition = new Vector3(gunParentedEffect.originalPosition.x, gunParentedEffect.originalPosition.y, gunParentedEffect.originalPosition.z);
    }

}
