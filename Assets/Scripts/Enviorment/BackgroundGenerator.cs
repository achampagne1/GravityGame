using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    [SerializeField] float scale = 20f;

    [SerializeField] float offsetX = 100f;
    [SerializeField] float offsetY = 100f;
    [SerializeField] Transform followPoint;

    private Vector3 lastPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        followPoint = MainCameraChecker.mainCameraLocation;
        lastPlayerPosition = followPoint.position;

        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        Texture2D noiseTexture = GenerateTexture();
        Sprite sprite = Sprite.Create(
            noiseTexture,
            new Rect(0, 0, noiseTexture.width, noiseTexture.height),
            new Vector2(0.5f, 0.5f),
            100f  // Pixels per unit — adjust based on your camera zoom
        );
        transform.localScale = new Vector3(100f, 100f, 0f);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    void Update()
    {
        followPoint = MainCameraChecker.mainCameraLocation;
        Vector3 delta = followPoint.position - lastPlayerPosition;
        transform.position += delta;
        lastPlayerPosition = followPoint.position;
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = new Color(sample, sample, sample, 1f);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }
}
