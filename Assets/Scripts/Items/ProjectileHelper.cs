using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note:
 * This class is intended to be the "parent" to all projectiles
 * this kind of gets around no multiple inheritence 
 * if a change with a projectile needs to be made (for bullets or laser) verify if it should be here 
 */

public interface IProjectile
{
    float getDamage();
    int getShotBy();
    
}
public struct ProjectileHelper
{
    //variables
    int shotBy;
    float damage;
    GameObject gameObject;

    public ProjectileHelper(int shotBy, float damage, GameObject gameObject)
    {
        gameObject.tag = "Projectile";
        this.shotBy = shotBy;
        this.damage = damage;
        this.gameObject = gameObject;
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

    public int getShotBy()
    {
        return shotBy;
    }

    public float getDamage()
    {
        return damage;
    }
}
