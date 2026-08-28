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
    [SerializeField] private float greenTeamGain = .2f;
    [SerializeField] private float bugGain = .5f;
    [SerializeField] private float droneGain = .5f;
    [SerializeField] private float greenTeamStart = .2f;
    [SerializeField] private float bugStart = .7f;
    [SerializeField] private float droneStart = .1f;


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
        double randomNum = (double)(UnityEngine.Random.Range(1, 101))/100f;

        double t = 1.0 - Math.Exp(-level / 10.0);

        double droneChance = droneStart + t * droneGain;

        double bugChance = 1.0 - droneChance - greenTeamStart;

        if (randomNum < bugChance)
            return 0;
        else if (randomNum < bugChance + greenTeamStart)
            return 1;
        else 
            return 2;
    }
}
