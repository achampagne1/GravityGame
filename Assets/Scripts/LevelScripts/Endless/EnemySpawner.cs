using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static UnityEditor.FilePathAttribute;

public class EnemySpawner : MonoBehaviour
{

    //serializedFields
    [SerializeField] private GameObject greenTeam;
    [SerializeField] private GameObject bug;
    [SerializeField] private GameObject drone;
    [SerializeField] private GameObject enemiesFolder;
    [SerializeField] private GameObject planetsFolder;
    [SerializeField] private float levelShift = .5f;
    [SerializeField] private float rampIn = .5f;
    [SerializeField] private float greenTeamGain = .2f;
    [SerializeField] private float spawnOffset = 3f;
    [SerializeField] private int spawnRate = 1;
    [SerializeField] private int level = 0;


    //private variables
    private int levelLatch = 0;
    private Dictionary<int,GameObject> enemies = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        levelLatch = level;
        enemies.Add(0, bug);
        enemies.Add(1, greenTeam);
        enemies.Add(2, drone);
    }

    // Update is called once per frame
    void Update()
    {
        if (levelLatch != level)
        {
            for (int i = 0; i < 5+(spawnRate*level); i++)
            {
                GameObject clone = spawnEnemy(level);
            }
            levelLatch = level;
        }
    }

    private GameObject spawnEnemy(int level)
    {
        int spawnChoice = spawnChance(level);
        GameObject clone = Instantiate(enemies[spawnChoice]);
        clone.transform.SetParent(enemiesFolder.transform);
        int planetChoice = UnityEngine.Random.Range(0, planetsFolder.transform.childCount);
        Transform planet = planetsFolder.transform.GetChild(planetChoice);
        SpriteRenderer sr = planet.GetComponent<SpriteRenderer>();
        Bounds planetBounds = sr.bounds;
        float planetRadius = planetBounds.extents.magnitude + spawnOffset + (spawnChoice == 2 ? 10 : 0);
        float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        clone.transform.position = (Vector2)planet.position + randomDirection * planetRadius;

        return clone;
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

        if (randomNum < bugChance)
            return 0;
        else if (randomNum < bugChance + greenTeamChance)
            return 1;
        else 
            return 2;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    public int getLevel() {
        return this.level;
    }
}
