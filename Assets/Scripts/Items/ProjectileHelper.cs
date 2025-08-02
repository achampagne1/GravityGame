using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note:
 * This class is intended to be the "parent" to all projectiles
 * this kind of gets around no multiple inheritence 
 * if a change with a projectile needs to be made (for bullets or laser) verify if it should be here 
 */
public struct ProjectileHelper:IProjectileInfo,IDamager
{
    //variables
    private int shotBy;
    private GameObject gameObject;
    private float damageVariable;

    public ProjectileHelper(int shotBy, float damageVariable, GameObject gameObject)
    {
        gameObject.tag = "Projectile";
        this.shotBy = shotBy;
        this.gameObject = gameObject;
        this.damageVariable = damageVariable;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != shotBy)
            Object.Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.GetComponent<TriggerBoundaryCotroller>().getLayerConnectedTo() != shotBy)
            Object.Destroy(gameObject);
    }

    public float damage(GameObject hitGameObject)
    {
        if (shotBy == hitGameObject.layer)
        {
            return 0f;
        }
        else
            return damageVariable;
    }

    public int getShotBy()
    {
        return shotBy;
    }

    public float getDamage()
    {
        return damageVariable;
    }
}
