using UnityEngine;

public class GunParentedEffect : MonoBehaviour, IParentedEffect
{
    public void parentedEffect()
    {
        Debug.Log("shoot");
    }
}
