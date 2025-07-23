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

    //used to abstract some of the smaller stuff for changing opacity
    public static void changeOpacity(SpriteRenderer sr,float opacity)
    {
        Color color = sr.color;
        color.a = opacity;
        sr.color = color;
    }

    //can be used to find the intersection between a chord and a circle. good for finding strile locations for bulelts
    //written by chat gpt
    public static bool chordIntersection(Vector2 p1, Vector2 p2, Vector2 circleCenter, float radius, out Vector2 intersection) 
    {
        intersection = Vector2.zero;

        Vector2 d = p2 - p1;                   // Direction vector of the line
        Vector2 f = p1 - circleCenter;         // Vector from circle center to p1

        float a = Vector2.Dot(d, d);
        float b = 2 * Vector2.Dot(f, d);
        float c = Vector2.Dot(f, f) - radius * radius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No intersection
            return false;
        }

        discriminant = Mathf.Sqrt(discriminant);

        float t1 = (-b - discriminant) / (2 * a);
        float t2 = (-b + discriminant) / (2 * a);

        bool found = false;

        if (t1 >= 0 && t1 <= 1)
        {
            intersection = p1 + t1 * d;
            found = true;
        }
        else if (t2 >= 0 && t2 <= 1)
        {
            intersection = p1 + t2 * d;
            found = true;
        }

        return found;
    }
}
