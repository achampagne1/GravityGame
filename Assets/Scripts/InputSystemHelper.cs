using UnityEngine;
using UnityEngine.InputSystem.Controls;

public struct InputSystemHelper
{
    private float lastKeyPress;
    private KeyControl key;

    public InputSystemHelper(KeyControl key)
    {
        this.key = key;
        this.lastKeyPress = 0f;
    }

    public bool wasPressedWithCooldown()
    {
        if (key.wasPressedThisFrame || (key.isPressed && Time.unscaledTime - lastKeyPress > 0.15f))
        {
            lastKeyPress = Time.unscaledTime;
            return true;
        }
        return false;
    }
}