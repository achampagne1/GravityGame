using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : ObjectController
{
    //object creation
    GameObject zombieObject;
    Timer timer;

    //public variables
    public float spawnRate = 3f;

    public override void Start()
    {
        base.Start();
        zombieObject = GameObject.Find("SpaceZombie");
        timer = new Timer(spawnRate);
        timer.startTimer();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        /*if (timer.checkTimer())
        {
            if (GameObject.FindGameObjectsWithTag("SpaceZombie(Clone").Length <= 10)
            {
                GameObject newZombie = Instantiate(zombieObject, transform.position, Quaternion.identity);
                newZombie.GetComponent<SpaceZombieController>().newInstance();
            }
            timer.startTimer();
        }*/
    }
}
