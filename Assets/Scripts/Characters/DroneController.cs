using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : CharacterController
{
    // Start is called before the first frame update
    void Start()
    {
        calculateCharacterStart();
    }

    // Update is called once per frame
    void Update()
    {
        calculateCharacterUpdate();
    }

    public override void hit(Transform transform)
    {
        if (invincibleFlag)
            return;

        //Note: transform is for the bullet, gameObject.transform is for the palyer
        bulletStrikeLocation = transform.position;
        health = health - 1f;
        SoundManager.instance.playSound(hitSound, transform, 1f);
        StartCoroutine(changeColorWrapper());
        //StartCoroutine(knockBack(leftStrikeLocation(transform)));

        //needs to be different for drone
        /*IEnumerator knockBack(bool strikeLeft)
        {
            forceLocalAdded = true;
            forceLocal = HelperFunctions.rotateVector(new Vector2(strikeLeft ? 3f : -3f, 3f), gameObject.transform.eulerAngles.z);
            yield return new WaitForSeconds(knockBackDuration);
            forceLocalAdded = false;
            yield return null;
        }*/

        IEnumerator changeColorWrapper()
        {
            animator.enabled = false;
            spriteRenderer.sprite = hitSprite;

            changeColorRecursive(gameObject, "#FF0000");
            yield return new WaitForSeconds(.05f);
            changeColorRecursive(gameObject, "#FFFFFF");
            animator.enabled = true;
            yield return null;
        }

        void changeColorRecursive(GameObject gameObjectChild, string color)
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

        /*bool leftStrikeLocation(Transform transform)
        {
            //determines if the bullet struck the characters left or right side
            Vector3 localPos = gameObject.transform.InverseTransformPoint(transform.position);
            localPos.x = facingLeft ? localPos.x * -1 : localPos.x;
            return localPos.x < 0;
        }*/
    }
    protected override IEnumerator die()
    {
        Transform gun = transform.Find("Gun");
        Destroy(gun.gameObject);
        base.die();
        yield return null;
    }
}
