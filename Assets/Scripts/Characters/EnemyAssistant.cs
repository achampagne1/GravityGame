using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EnemyAssistant
{
    private float angleOffsetSetting = 30; //one day move this out so it can be a serialized field
    private bool playerFound;
    private int angle = 0;
    private int resolution = 4; //resolution is used to determine how many degrees seperate each ray cast shoot. it should not be a number 90 is divisible by
    private Vector2 direction = Vector2.zero;
    private GameObject gameObject;
    private GameObject player = null;
    private static readonly ContactFilter2D filter = getContactFilter2D();
    public EnemyAssistant(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public Vector3 detectPlayer(bool facingLeft)
    {
        //TODO: implement a viewing window bigger than 90 degrees
        bool validDirection = false;
        if (player != null)
        {
            direction = (player.transform.position - gameObject.transform.position).normalized;
            Vector2 localDirection = gameObject.transform.InverseTransformDirection(direction);

            float angleOffset = (facingLeft ? -angleOffsetSetting : angleOffsetSetting) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleOffset);
            float sin = Mathf.Sin(angleOffset);
            Vector2 rotatedDir = new Vector2(
                localDirection.x * cos - localDirection.y * sin,
                localDirection.x * sin + localDirection.y * cos
            );

            if (!facingLeft && rotatedDir.x > 0 && rotatedDir.y > 0)
                validDirection = true;
            else if (facingLeft && rotatedDir.x < 0 && rotatedDir.y > 0)
                validDirection = true;
        }

        if (!validDirection)
        {
            float angleToCast = gameObject.transform.eulerAngles.z + angle;
            angleToCast += facingLeft ? 90+ angleOffsetSetting : -angleOffsetSetting;
            direction.x = Mathf.Cos(angleToCast * Mathf.Deg2Rad);
            direction.y = Mathf.Sin(angleToCast * Mathf.Deg2Rad);
            angle = (angle + resolution) % 91;
        }

        RaycastHit2D[] hits = new RaycastHit2D[10];
        int hitCount = Physics2D.Raycast(gameObject.transform.position, direction, filter, hits, 30f);

        for (int i = 0; i < hitCount; i++)
        {
            GameObject hitObj = hits[i].collider.gameObject;

            if (hitObj != gameObject && hitObj.layer == LayerMask.NameToLayer("player"))
            {
                player = hitObj;
                return gameObject.transform.InverseTransformDirection((hitObj.transform.position- gameObject.transform.position).normalized); //for now it will just be a direciton vector
            }

            if (hitObj.layer != LayerMask.NameToLayer("player")) { }
                break;
        }
        player = null;
        return new Vector3(0f,0f,1f);
    }

    public void setAngleOffsetSetting(float angleOffsetSetting)
    {
        this.angleOffsetSetting = angleOffsetSetting;
    }

    private static ContactFilter2D getContactFilter2D() {
        int enemyLayer = LayerMask.NameToLayer("enemy");
        int triggerLayer = LayerMask.NameToLayer("TriggerBoudary");
        int itemsLayer = LayerMask.NameToLayer("items");

        int everythingMask = Physics2D.AllLayers;
        int mask = everythingMask & ~(1 << enemyLayer) & ~(1 << triggerLayer) & ~(1 << itemsLayer);

        ContactFilter2D f = new ContactFilter2D();
        f.SetLayerMask(mask);
        f.useTriggers = false;
        return f;
    }
}