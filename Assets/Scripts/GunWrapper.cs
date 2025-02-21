using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWrapper : MonoBehaviour
{
    //object creation
    GameObject bulletObject;
    public void Start()
    {
        bulletObject = GameObject.Find("Bullet");
    }

    void Update()
    {
        
    }

    public void shoot(Vector3 location,Vector3 direction)
    {
        GameObject ShotBullet = Instantiate(bulletObject, location, Quaternion.identity);
        ShotBullet.GetComponent<BulletController>().newInstance(direction);
        ShotBullet.GetComponent<BulletController>().Start();
    }
}
