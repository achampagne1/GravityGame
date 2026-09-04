using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Recorder.OutputPath;

public class PropPlacer : MonoBehaviour
{
    [SerializeField] int amount = 20;
    [SerializeField] int maxTries = 20;
    [SerializeField] float offset = .1f;
    [SerializeField] List<Sprite> props = new List<Sprite>();
    private List<Vector2> locations = new List<Vector2>();
    private List<GameObject> objects = new List<GameObject>();
    private PolygonCollider2D collider;
    void Start()
    {
        System.Random rand = new System.Random();
        collider = GetComponent<PolygonCollider2D>();
        for (int i = 0; i < collider.pathCount; i++)
        {
            Vector2[] points = collider.GetPath(i);
            foreach (Vector2 point in points)
            {
                locations.Add(collider.transform.TransformPoint(point));
            }
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject prop = new GameObject("prop");
            prop.layer = LayerMask.NameToLayer("prop");
            SpriteRenderer sr = prop.AddComponent<SpriteRenderer>();
            int depth = rand.Next(-90, -85);
            sr.sortingOrder = depth;

            float t = Mathf.InverseLerp(-90, -85, depth); 
            float brightness = Mathf.Lerp(0.8f, 1f, t); 

            Color originalColor = sr.color;
            sr.color = new Color(
                originalColor.r * brightness,
                originalColor.g * brightness,
                originalColor.b * brightness,
                originalColor.a
            );

            prop.transform.parent = transform.Find("Props");

            int randomNumber = rand.Next(props.Count);
            sr.sprite = props[randomNumber];

            prop.transform.localScale = prop.transform.localScale * (0.01f* rand.Next(40, 50));
            if (rand.Next(2) == 0)
            {
                Vector3 scale = prop.transform.localScale;
                scale.x *= -1;
                prop.transform.localScale = scale;
            }

            Vector2 location = new Vector2();
            int tries = 0;
            bool intersect = true;
            do
            {
                randomNumber = rand.Next(locations.Count);
                location = locations[randomNumber];


                Vector2 direction = ((Vector2)transform.position - location).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                prop.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90);

                float height = sr.sprite.bounds.size.y * prop.transform.localScale.y;
                prop.transform.position = location + (Vector2)prop.transform.up * (height - offset);
                intersect = false;
                foreach (GameObject placedProp in objects)
                {
                    if (sr.bounds.Intersects(placedProp.GetComponent<SpriteRenderer>().bounds))
                    {
                        intersect = true;
                        tries++;
                        break;
                    }
                }
            } while (intersect&&tries<maxTries);

            locations.RemoveAt(randomNumber);
            objects.Add(prop);
        }
    }
}
