using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : CharacterController
{
    private EnemyAssistant[] enemyAssistants = new EnemyAssistant[5];
    private GameObject droneGun;
    private DroneGunController droneGunController;
    private Vector3 playerNotFound = new Vector3(0f, 0f, 1f);
    private Vector2 center = Vector2.zero;

    [SerializeField] float width = 3f;  
    [SerializeField] float height = .2f;  
    [SerializeField] float speed = 1f;   
    private float hoverTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        calculateCharacterStart();
        center = (Vector2)transform.position;
        droneGun = transform.Find("Gun").gameObject;
        droneGunController = droneGun.GetComponent<DroneGunController>();
        int angleOffset = 0;
        for (int i = 0; i < enemyAssistants.Length; i++)
        {
            enemyAssistants[i] = new EnemyAssistant(gameObject);
            enemyAssistants[i].setAngleOffsetSetting(angleOffset);
            angleOffset += 72;
        }
    }

    // Update is called once per frame
    void Update()
    {
        hover();
        Vector3 playerPos = detectPlayerWrapper();
        if (playerPos != playerNotFound)
        {
            float angleRad = Mathf.Atan2(playerPos.y, playerPos.x);
            float angleDeg = angleRad * Mathf.Rad2Deg;
            droneGun.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg)*transform.rotation;
            droneGunController.setPlayerSeen(true);
        }
        else
            droneGunController.setPlayerSeen(false);
        calculateCharacterUpdate();
    }

    public void hover()
    {
        hoverTime = (hoverTime +(Time.deltaTime * speed))%(2*Mathf.PI);
        float x = width * Mathf.Sin(hoverTime);
        float y = height * Mathf.Sin(2 * hoverTime);

        transform.position = new Vector3(center.x + x, center.y + y, transform.position.z);
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

    private Vector3 detectPlayerWrapper()
    {
        for (int i = 0; i < enemyAssistants.Length; i++)
        {
            Vector3 temp = enemyAssistants[i].detectPlayer(false);
            if (temp != playerNotFound)
                return temp;
        }
        return playerNotFound;
    }
}
