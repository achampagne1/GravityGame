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
        this.maxRotationalAngle = maxRotationalAngle;
        recoil(float1);
    }

    protected void recoil(float magnitude)
    {
        //originalPosition = new Vector3(armLengthActive, 0, 0);
        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        Vector3 previousDirection = inputDirection;
        float inputAngle = Mathf.Atan2(previousDirection.y, previousDirection.x) * Mathf.Rad2Deg;
        float recoilAngle = inputAngle + magnitude * recoilMultiplier * facingLeftInt;
        Vector3 recoilDirection = Quaternion.Euler(0f, 0f, recoilAngle) * Vector3.right;
        recoilCoroutine = StartCoroutine(recoilRoutine(previousDirection, recoilDirection, magnitude));
    }

    protected IEnumerator recoilRoutine(Vector3 previousDirection, Vector3 recoilDirection, float magnitude)
    {
        float elapsedTime = 0f;
        float kickDuration = recoilSpeedMultiplier * magnitude;

        while (elapsedTime < kickDuration)
        {
            inputDirection = Vector3.Lerp(previousDirection, recoilDirection, elapsedTime / kickDuration);
            HelperFunctions.rotateAboutPoint(prop, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < recoilReturnSpeed)
        {
            inputDirection = Vector3.Lerp(recoilDirection, previousDirection, elapsedTime / recoilReturnSpeed);
            HelperFunctions.rotateAboutPoint(prop, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        inputDirection = previousDirection;
        HelperFunctions.rotateAboutPoint(prop, inputDirection, originalPosition, facingLeftInt, maxRotationalAngle);
        recoilCoroutine = null;
    }

    public void copyData(IParentedEffect otherEffect)
    {
        GunParentedEffect gunParentedEffect = otherEffect as GunParentedEffect;
        recoilMultiplier = gunParentedEffect.recoilMultiplier;
        recoilSpeedMultiplier = gunParentedEffect.recoilSpeedMultiplier;
        recoilReturnSpeed = gunParentedEffect.recoilReturnSpeed;
        float1 = gunParentedEffect.float1;
        facingLeftInt = gunParentedEffect.facingLeftInt;
        inputDirection = new Vector3(gunParentedEffect.inputDirection.x, gunParentedEffect.inputDirection.y, gunParentedEffect.inputDirection.z);
        originalPosition = new Vector3(gunParentedEffect.originalPosition.x, gunParentedEffect.originalPosition.y, gunParentedEffect.originalPosition.z);
        //prop = new Transform(gunParentedEffect.prop);
    }

}
