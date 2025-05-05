using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Put this anywhere in the scene - on the camera, on a seperate game object, it doesn't matter
public class StarGenerator : MonoBehaviour
{
    [SerializeField] Transform _starsInstantiatePoint; //This needs to be a game object that is set to a position of 0, 0, 100
    [SerializeField] List<Sprite> _starSprites; //Populate this with the three star sprites 
    [SerializeField] List<Sprite> planetSprites;
    [SerializeField] Vector2 _minMaxSize = new Vector2(0.05f, 1); //This sets how small/big the stars can be
    [SerializeField] Vector2 _mapSize = new Vector2(1000, 1000); //This sets the size of the background
    [SerializeField] int _count = 2000; //This sets how many stars you want to spawn
    [SerializeField] int planetCount = 10;
    [SerializeField] Transform spawnPoint;
    private GameObject initialStar;

    void Start()
    {
        initialStar = transform.GetChild(0).gameObject; //initial star must be first child
        //planets copy form initial star too
        createStars();
        createPlanets();
    }

    private void createStars()
    {
        for (int i = 0; i < _count; i++)
        {
            float alpha = Random.Range(0.25f, 1);
            GameObject starObject = Instantiate(initialStar, new Vector3(Random.Range(spawnPoint.position.x-_mapSize.x, spawnPoint.position.x+_mapSize.x), Random.Range(spawnPoint.position.y - _mapSize.y, spawnPoint.position.y+_mapSize.y), 0), Quaternion.identity);
            starObject.tag = "twinkle";
            SpriteRenderer spriteRenderer = starObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _starSprites[Random.Range(0, _starSprites.Count)];
            spriteRenderer.sortingOrder = -102;
            spriteRenderer.color = new Color(1, 1, 1, alpha);

            float scale = Random.Range(_minMaxSize.x, _minMaxSize.y);
            starObject.transform.SetParent(_starsInstantiatePoint);
            starObject.transform.localScale = new Vector3(scale, scale, 1);
            if (Random.Range(0, 2) == 0) 
                starObject.transform.localEulerAngles = new Vector3(0, 0, 45);

        }
    }
    private void createPlanets()
    {
        for (int i = 0; i < planetCount; i++)
        {
            GameObject planetObject = Instantiate(initialStar, new Vector3(Random.Range(spawnPoint.position.x - _mapSize.x, spawnPoint.position.x + _mapSize.x), Random.Range(spawnPoint.position.y - _mapSize.y, spawnPoint.position.y + _mapSize.y), 0), Quaternion.identity);
            float paralax = Random.Range(.95f, .99f);
            planetObject.GetComponent<StarTwinkle>().setScale(paralax);
            SpriteRenderer planetRenderer = planetObject.GetComponent<SpriteRenderer>();
            planetRenderer.sprite = planetSprites[Random.Range(0, planetSprites.Count)];
            int order = (int)(paralax * -100);
            planetRenderer.sortingOrder = order;

            float scale = Random.Range(.1f, .25f); 
            planetObject.transform.SetParent(_starsInstantiatePoint);
            planetObject.transform.localScale = new Vector3(scale, scale, 1);
            if (Random.Range(0, 2) == 0)
                planetObject.transform.localEulerAngles = new Vector3(0, 0, 45);

        }
    }
    public void setSpawnPoint(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }
}