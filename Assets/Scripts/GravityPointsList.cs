using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPointsList : MonoBehaviour
{
    public List<GameObject> gravityPoints = new List<GameObject>();
    public List<Color> colors = new List<Color>
    {
        new Color(1f, 0f, 0f),   // Pure Red
        new Color(1f, 0.5f, 0f), // Vivid Orange
        new Color(1f, 1f, 0f),   // Bright Yellow
        new Color(0f, 1f, 0f),   // Neon Green
        new Color(0f, 1f, 1f),   // Cyan
        new Color(0f, 0.5f, 1f), // Electric Blue
        new Color(0.5f, 0f, 1f), // Deep Purple
        new Color(1f, 0f, 1f),   // Hot Pink
        new Color(1f, 0f, 0.5f), // Vibrant Magenta
        new Color(0.5f, 1f, 0f)  // Lime Green
    };

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            gravityPoints.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        gravityPoints.Clear();
        foreach (Transform child in transform)
        {
            gravityPoints.Add(child.gameObject);
        }

        if (gravityPoints.Count == 0) return; // Exit if no gravity points exist

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        float spacing = 12f;

        // Cache gravity point positions & field sizes
        Dictionary<GameObject, (Vector3 position, float fieldSize)> gravityData = new Dictionary<GameObject, (Vector3, float)>();
        List<Color> fieldColors = new List<Color>();

        for (int i = 0; i < gravityPoints.Count; i++)
        {
            GameObject gravityPoint = gravityPoints[i];
            GravityPointController gravityPointController = gravityPoint.GetComponent<GravityPointController>();
            if (gravityPointController != null)
            {
                gravityData[gravityPoint] = (gravityPoint.transform.position, gravityPointController.getFieldSize());
                fieldColors.Add(colors[i % colors.Count]);
            }
        }

        // Loop through screen space with specified spacing
        for (int x = 0; x < screenWidth; x += (int)spacing)
        {
            for (int y = 0; y < screenHeight; y += (int)spacing)
            {
                // Convert screen point to world position
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.nearClipPlane));

                Gizmos.color = Color.yellow;
                float closestGravityField = float.MaxValue;
                int closestIndex = -1;

                // Find the closest gravity point
                for (int i = 0; i < gravityPoints.Count; i++)
                {
                    GameObject gravityPoint = gravityPoints[i];
                    var (position, fieldSize) = gravityData[gravityPoint];

                    float adjustedDistance = (worldPoint - position).magnitude / fieldSize;
                    if (adjustedDistance < closestGravityField)
                    {
                        closestGravityField = adjustedDistance;
                        closestIndex = i;
                    }
                }

                if (closestIndex != -1)
                {
                    Gizmos.color = fieldColors[closestIndex];
                    Gizmos.DrawSphere(worldPoint, 0.0625f);
                }
            }
        }
    }
}
