using System.Collections;
using UnityEngine;

public class RecoilEffect : MonoBehaviour, IItemEffect
{
    [SerializeField] private float recoilMultiplier = 100f;
    [SerializeField] private float recoilSpeedMultiplier = 1.0f;
    [SerializeField] private float recoilReturnSpeed = .2f;

    public float float1 { get; set; }
    public Vector3 inputDirection { get; set; }
    public Vector3 originalPosition { get; set; }
    public Vector2 maxRotationalAngle { get; set; }
    public Vector3 point { get; set; }
    public ILeft left { get; set; }

    private Coroutine recoilCoroutine;
    private float recoilOffsetAngle;

    public void effect(Vector3 inputDirection, Vector3 originalPosition, Vector2 maxRotationalAngle, Vector3 point = default(Vector3))
    {
        this.inputDirection = inputDirection;
        this.originalPosition = originalPosition;
        this.maxRotationalAngle = maxRotationalAngle;
        this.point = point;
        
        recoil(inputDirection, float1);
    }

    public void recoil(Vector3 baseDirection, float magnitude)
    {
        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        float startingOffset = recoilOffsetAngle;
        float targetOffset = startingOffset + magnitude * recoilMultiplier * left.facingLeftInt;
        recoilCoroutine = StartCoroutine(recoilRoutine(baseDirection, startingOffset, targetOffset, magnitude));
    }

    private IEnumerator recoilRoutine(Vector3 baseDirection, float startingOffset, float targetOffset, float magnitude)
    {
        float elapsedTime = 0f;
        float kickDuration = recoilSpeedMultiplier * magnitude;

        while (elapsedTime < kickDuration)
        {
            lerpHelperFunc(startingOffset, targetOffset, kickDuration, ref elapsedTime, baseDirection);
            yield return null;
        }

        recoilOffsetAngle = targetOffset;
        elapsedTime = 0f;
        while (elapsedTime < recoilReturnSpeed)
        {
            lerpHelperFunc(targetOffset, 0f, recoilReturnSpeed, ref elapsedTime, baseDirection);
            yield return null;
        }

        recoilOffsetAngle = 0f;
        inputDirection = baseDirection;
        HelperFunctions.rotateAboutPoint(transform, inputDirection, originalPosition, left.facingLeftInt, maxRotationalAngle, point);
        recoilCoroutine = null;
    }

    private void lerpHelperFunc(float start, float end,float speed,ref float elapsedTime, Vector3 baseDirection) {
        recoilOffsetAngle = Mathf.Lerp(start, end, elapsedTime / speed);
        inputDirection = Quaternion.Euler(0f, 0f, recoilOffsetAngle) * baseDirection;
        HelperFunctions.rotateAboutPoint(transform, inputDirection, originalPosition, left.facingLeftInt, maxRotationalAngle, point);
        elapsedTime += Time.deltaTime;
    }

    public void copyData(IItemEffect otherEffect)
    {
        RecoilEffect gunParentedEffect = otherEffect as RecoilEffect;
        recoilMultiplier = gunParentedEffect.recoilMultiplier;
        recoilSpeedMultiplier = gunParentedEffect.recoilSpeedMultiplier;
        recoilReturnSpeed = gunParentedEffect.recoilReturnSpeed;
        float1 = gunParentedEffect.float1;
        inputDirection = new Vector3(gunParentedEffect.inputDirection.x, gunParentedEffect.inputDirection.y, gunParentedEffect.inputDirection.z);
        originalPosition = new Vector3(gunParentedEffect.originalPosition.x, gunParentedEffect.originalPosition.y, gunParentedEffect.originalPosition.z);
    }

}
