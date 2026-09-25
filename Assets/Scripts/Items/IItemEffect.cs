using UnityEngine;

public interface IItemEffect
{
    public float float1 {get; set;}
    public Vector3 inputDirection { get; set; }
    public Vector3 originalPosition { get; set; }
    public Vector2 maxRotationalAngle { get; set; }
    public ILeft left { get; set; }

    public void onConnect(Transform transform)
    {
        left = transform.GetComponent<ILeft>();
    }

    public void onDisconnect()
    {

    }

    public void effect(Vector3 inputDirection, Vector3 originalPosition, Vector2 maxRotationalAngle, Vector3 point = default(Vector3));

    public void copyData(IItemEffect otherEffect);
}
