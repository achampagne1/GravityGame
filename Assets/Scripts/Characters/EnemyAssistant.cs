using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

class EnemyAssistant
{
    private float angleOffsetSetting = 30; //one day move this out so it can be a serialized field
    private bool playerFound;
    private int angle = 0;
    private int resolution = 4; //resolution is used to determine how many degrees seperate each ray cast shoot. it should not be a number 90 is divisible by
    private Vector2 direction = Vector2.zero;
    private GameObject gameObject;
    private GameObject player;
    private static readonly ContactFilter2D filter = getContactFilter2D();
    public EnemyAssistant(GameObject gameObject,GameObject player)
    {
        this.gameObject = gameObject;
        this.player = player;
    }

    public Vector3 detectPlayer(bool facingLeft)
    {
        //TODO: implement a viewing window bigger than 90 degrees
        bool validDirection = false;
        if (player != null)
        {
            Vector3 playerDirection = (player.transform.position-gameObject.transform.position).normalized;
            Debug.DrawRay(gameObject.transform.position, playerDirection * 30f, Color.red);
            RaycastHit2D[] hits = new RaycastHit2D[1];

            int count = Physics2D.Raycast(gameObject.transform.position, playerDirection, filter, hits, 30f);
            if (count > 0)
            {
                if (hits[0].collider.gameObject.layer == LayerMask.NameToLayer("player"))
                    Debug.Log("See player");
            }
        }

        if (!validDirection)
        {
            float angleToCast = gameObject.transform.eulerAngles.z + angle;
            angleToCast += facingLeft ? 90+ angleOffsetSetting : -angleOffsetSetting;
            direction.x = Mathf.Cos(angleToCast * Mathf.Deg2Rad);
            direction.y = Mathf.Sin(angleToCast * Mathf.Deg2Rad);
            angle = (angle + resolution) % 91;
        }
        return new Vector3(0f,0f,1f);
    }

    public void setAngleOffsetSetting(float angleOffsetSetting)
    {
        this.angleOffsetSetting = angleOffsetSetting;
    }

    private static ContactFilter2D getContactFilter2D() {
        int enemyLayer = LayerMask.NameToLayer("enemy");
        int triggerLayer = LayerMask.NameToLayer("trigger");
        int itemsLayer = LayerMask.NameToLayer("item");
        int backgroundLayer = LayerMask.NameToLayer("background");

        int everythingMask = Physics2D.AllLayers;
        int mask = everythingMask & ~(1 << enemyLayer) & ~(1 << triggerLayer) & ~(1 << itemsLayer) & ~(1 << backgroundLayer);

        ContactFilter2D f = new ContactFilter2D();
        f.SetLayerMask(mask);
        f.useTriggers = false;
        return f;
    }
}