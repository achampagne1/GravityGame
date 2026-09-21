using UnityEngine;

public interface IParentedEffect
{
    public float float1 {get; set;}
    public int facingLeftInt { get; set; }
    public Vector3 inputDirection { get; set; }
    public Vector3 originalPosition { get; set; }
    public Vector2 maxRotationalAngle { get; set; }
    public Transform prop { get; set; }

    public void parentedEffect(Transform prop, int facingLeftInt, Vector3 inputDirection, Vector3 originalPosition, Vector2 maxRotationalAngle);

    public void copyData(IParentedEffect otherEffect);
}
