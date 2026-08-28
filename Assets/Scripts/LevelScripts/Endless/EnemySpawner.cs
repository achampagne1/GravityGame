using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //publicly accessible variables
    [SerializeField] public int level;

    //serializedFields
    [SerializeField] private GameObject greenTeam;
    [SerializeField] private GameObject bug;
    [SerializeField] private GameObject drone;
    [SerializeField] private float levelShift = .5f;
    [SerializeField] private float rampIn = .5f;
    [SerializeField] private float greenTeamGain = .2f;


    //private variables
    private int levelLatch = 0;

    // Start is called before the first frame update
    void Start()
    {
        levelLatch = level;
    }

    // Update is called once per frame
    void Update()
    {
        if (levelLatch != level)
        {

            for (int i = 0; i < 5; i++)
            {
                if (spawnChance(level) == 0)
                {
                    GameObject bugClone = Instantiate(bug);
                }
                else if (spawnChance(level) == 1)
                {
                    GameObject greenTeamClone = Instantiate(greenTeam);
                }
                else
                {
                    GameObject droneClone = Instantiate(drone);
                }
            }
            
            //choose a random number
            levelLatch = level;
        }
    }

    private int spawnChance(int level)
    {
        //spawn rate is an s curve
        //levelshift adjusts the cross over point between drones and bugs
        //rampin defines how lazily it reaches that cross over
        //greenTeamChance is constant
        double randomNum = (double)(UnityEngine.Random.Range(1, 101))/100f;
        double greenTeamChance = greenTeamGain;
        double droneChance = 1/(1+ Math.Exp(levelShift*rampIn-(rampIn/10)*level));
        double bugChance = 1 - droneChance;
        droneChance *= 1-greenTeamChance;
        bugChance *= 1 - greenTeamChance;
        //make 3 bands, one for bug, one for green, one for drone
        //chances should always add up to 1

        if (randomNum < bugChance)
            return 0;
        else if (randomNum < bugChance + greenTeamChance)
            return 1;
        else 
            return 2;
    }
}
