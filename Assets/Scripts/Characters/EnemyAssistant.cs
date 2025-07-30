using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EnemyAssistant
{
    private bool playerFound;
    private int angle = 0;
    private int resolution = 4; //resolution is used to determine how many degrees seperate each ray cast shoot. it should not be a number 90 is divisible by
    private Vector2 direction = Vector2.zero;
    private GameObject gameObject;
    private GameObject player = null;
    private ContactFilter2D filter;
    public EnemyAssistant(GameObject gameObject)
    {
        this.gameObject = gameObject;

        int enemyLayer = LayerMask.NameToLayer("enemy");
        int triggerLayer = LayerMask.NameToLayer("TriggerBoudary");
        int itemsLayer = LayerMask.NameToLayer("items");

        int everythingMask = Physics2D.AllLayers;

        int mask = everythingMask & ~(1 << enemyLayer) & ~(1 << triggerLayer) & ~(1 << itemsLayer);

        filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        filter.useTriggers = false;
    }
    public Vector3 detectPlayer(bool facingLeft)
    {
        bool validDirection = false;
        if (player != null)
        {
            direction = (player.transform.position - gameObject.transform.position).normalized;
            Vector2 localDirection = gameObject.transform.InverseTransformDirection(direction);
            if(!facingLeft && localDirection.x>0&&localDirection.y>0)
                validDirection= true;
            else if (facingLeft && localDirection.x < 0 && localDirection.y > 0)
                validDirection = true;
        }

        if (!validDirection)
        {
            float angleToCast = gameObject.transform.eulerAngles.z + angle;
            angleToCast += facingLeft ? 90 : 0;
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
}
/*
if (facingLeft && (direction.x > 0 || direction.y < 0))
{
    //NOTE: I know this code is duplicated. its to check if the direction si outside of the enemies "view"
    float angleToCast = gameObject.transform.eulerAngles.z + angle;
    angleToCast += 90;
    direction.x = Mathf.Cos(angleToCast * Mathf.Deg2Rad);
    direction.y = Mathf.Sin(angleToCast * Mathf.Deg2Rad);
    angle = (angle + resolution) % 91;
}
else if (!facingLeft && (direction.x < 0 || direction.y < 0))
{
    //NOTE: I know this code is duplicated. its to check if the direction si outside of the enemies "view"
    float angleToCast = gameObject.transform.eulerAngles.z + angle;
    direction.x = Mathf.Cos(angleToCast * Mathf.Deg2Rad);
    direction.y = Mathf.Sin(angleToCast * Mathf.Deg2Rad);
    angle = (angle + resolution) % 91;
}*/