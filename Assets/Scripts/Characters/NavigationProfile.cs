using UnityEngine;

[CreateAssetMenu(fileName = "NavigationProfile", menuName = "Scriptable Objects/NavigationProfile")]
public class NavigationProfile : ScriptableObject
{
    public bool canJump;
    public bool canFly;
    public bool canUseGravity;
    public bool avoidsHazards = true;

    public float movementSpeed = 3f;
    public float jumpHeight = 2f;

    public bool canTraverse(Vector2 from, Vector2 to)
    {

        return true;
    }
}
