using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Note:
 * This class is intended to be the "parent" to all projectiles
 * this kind of gets around no multiple inheritence 
 * if a change with a projectile needs to be made (for bullets or laser) verify if it should be here 
 */
public class ProjectileHelper:IProjectileInfo,IDamager
{
    //variables
    private int shotBy;
    private GameObject gameObject;
    private float damageVariable;
    private float lifeTimeVariable;

    //objects
    Timer lifeTime;

    public ProjectileHelper(int shotBy, float damageVariable, GameObject gameObject,float lifeTimeVariable)
    {
        gameObject.tag = "Projectile";
        this.shotBy = shotBy;
        this.gameObject = gameObject;
        this.damageVariable = damageVariable;
        this.lifeTimeVariable = lifeTimeVariable;
        if (lifeTimeVariable > 0)
        {
            lifeTime = new Timer(lifeTimeVariable);
            lifeTime.startTimer();
        }
        else
            lifeTime = new Timer(0);
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

    public void update()
    {
        if(lifeTimeVariable > 0&& lifeTime.checkTimer())
            Object.Destroy(gameObject);
    }

    public bool damage(GameObject hitGameObject)
    {

        if (shotBy == hitGameObject.layer|| (hitGameObject.layer == 14&&hitGameObject.GetComponent<TriggerBoundaryCotroller>().getLayerConnectedTo()==shotBy))
        {
            return false;
        }
        else
        {
            IHealth health = hitGameObject.GetComponent<IHealth>();
            health.setHealth(health.getHealth() - damageVariable);
            return true;
        }
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
