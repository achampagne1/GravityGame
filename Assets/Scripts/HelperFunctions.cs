using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HelperFunctions
{
    public static Vector2 rotateVector(Vector2 input, Vector2 rotateBy)
    {
        float angle = Mathf.Atan2(rotateBy.y, rotateBy.x); // Get angle in radians
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        float newX = input.x * cos - input.y * sin;
        float newY = input.x * sin + input.y * cos;

        return new Vector2(newX, newY);
    }

    public static Vector2 rotateVector(Vector2 input, float rotateBy)
    {
        float radians = rotateBy * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        float x = input.x * cos - input.y * sin;
        float y = input.x * sin + input.y * cos;

        return new Vector2(x, y);
    }

    public static Vector2 angleToDirection(float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}
