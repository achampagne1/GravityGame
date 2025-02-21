using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : ObjectController
{
    //object creation
    GameObject zombieObject;
    Timer timer;

    void Start()
    {
        calculateStart();
        zombieObject = GameObject.Find("SpaceZombie");
        timer = new Timer(1f);
        timer.startTimer();
    }

    // Update is called once per frame
    void Update()
    {
        calculateUpdate();
        print(GameObject.FindGameObjectsWithTag("SpaceZombie").Length);
        if (timer.checkTimer())
        {
            if (GameObject.FindGameObjectsWithTag("SpaceZombie").Length <= 10)
            {
                GameObject newZombie = Instantiate(zombieObject, transform.position, Quaternion.identity);
                newZombie.GetComponent<SpaceZombieController>().newInstance();
            }
            timer.startTimer();
        }
    }
}
