using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour,IProjectileInfo,IDamager
{
    //game variables
    [SerializeField] float damageVariable = 1f;
    [SerializeField] float lifetime = 5f;

    //object creation
    ProjectileHelper projectileHelper;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void init(int shotBy)
    {
        projectileHelper = new ProjectileHelper(shotBy,damageVariable,gameObject,lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        projectileHelper.update();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        projectileHelper.OnCollisionEnter2D(collision);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        projectileHelper.OnTriggerEnter2D(trigger);
    }

    public bool damage(GameObject hitGameobject)
    {
        return projectileHelper.damage(hitGameobject);
    }

    public int getShotBy()
    {
        return projectileHelper.getShotBy();
    }

    public float getDamage()
    {
        return damageVariable;
    }
}
