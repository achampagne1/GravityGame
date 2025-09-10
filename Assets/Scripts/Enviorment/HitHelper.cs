using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitHelper
{
    public static bool leftStrikeLocation(GameObject gameObject, Vector2 strikeLocation, bool facingLeft)
    {
        //determines if the bullet struck the characters left or right side
        Vector3 localPos = gameObject.transform.InverseTransformPoint(strikeLocation);
        localPos.x = facingLeft ? localPos.x * -1 : localPos.x;
        return localPos.x < 0;
    }
    public static void changeColorRecursive(GameObject gameObjectChild, string color) //this can be used in other places
    {
        //recursivley sets all children of the player to an opacity. could be used for other things too
        SpriteRenderer sr = gameObjectChild.GetComponent<SpriteRenderer>();
        if (sr != null && sr.color.a != 0f)
        {
            Color newColor;
            ColorUtility.TryParseHtmlString(color, out newColor);
            sr.color = newColor;
        }

        foreach (Transform child in gameObjectChild.transform)
        {
            changeColorRecursive(child.gameObject, color);
        }
    }

    public static IEnumerator changeColorWrapper(Animator animator, SpriteRenderer sr, Sprite hitSprite, GameObject gameObject)
    {
        animator.enabled = false;
        sr.sprite = hitSprite;

        changeColorRecursive(gameObject, "#FF0000");
        yield return new WaitForSeconds(.05f);
        changeColorRecursive(gameObject, "#FFFFFF");
        animator.enabled = true;
        yield return null;
    }
}
