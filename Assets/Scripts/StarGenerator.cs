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
    [SerializeField] Vector2 _minMaxSize = new Vector2(0.05f, 1); //This sets how small/big the stars can be
    [SerializeField] Vector2 _mapSize = new Vector2(1000, 1000); //This sets the size of the background
    [SerializeField] int _count = 2000; //This sets how many stars you want to spawn

    void Start()
    {
        CreateStars();
    }

    private void CreateStars()
    {
        for (int i = 0; i < _count; i++)
        {
            float alpha = Random.Range(0.25f, 1);
            GameObject starObject = new GameObject("Star");
            starObject.AddComponent<StarTwinkle>();
            SpriteRenderer spriteRenderer = starObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _starSprites[Random.Range(0, _starSprites.Count)];
            spriteRenderer.sortingOrder = -100;
            spriteRenderer.color = new Color(1, 1, 1, alpha);

            float scale = Random.Range(_minMaxSize.x, _minMaxSize.y);
            starObject.transform.SetParent(_starsInstantiatePoint);
            starObject.transform.localPosition = new Vector3(Random.Range(-_mapSize.x, _mapSize.x), Random.Range(-_mapSize.y, _mapSize.y), 0);
            starObject.transform.localScale = new Vector3(scale, scale, 1);
            if (Random.Range(0, 2) == 0) starObject.transform.localEulerAngles = new Vector3(0, 0, 45);

        }
    }
}