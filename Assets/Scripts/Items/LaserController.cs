using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{

    //object creation
    Timer lifetime = new Timer(5f);
    ProjectileHelper helper;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void init(int shotBy)
    {
        lifetime.startTimer();
        helper = new ProjectileHelper(shotBy,gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime.checkTimer())
            Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        helper.OnCollisionEnter2D(collision);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        helper.OnTriggerEnter2D(trigger);
    }

    public int getShotBy()
    {
        return helper.getShotBy();
    }
}
