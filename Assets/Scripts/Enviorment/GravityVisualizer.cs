using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GravityVisualizer : MonoBehaviour
{
    [SerializeField] private float spacing = 25f;
    [SerializeField] private GameObject planets;

    private List<GravityPoint> gravityPoints = new List<GravityPoint>();
    private Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

    private readonly List<Color> colors = new List<Color>
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

    void OnDrawGizmosSelected()
    {
        if (planets == null) return;

        constructData();

        int screenWidth = Screen.width*3;
        int screenHeight = Screen.height*3;

        Camera cam = Camera.main;
        if (cam == null) return;

        for (int x = 0; x < screenWidth; x += (int)spacing)
        {
            for (int y = 0; y < screenHeight; y += (int)spacing)
            {
                float distanceToWorldPlane = -cam.transform.position.z;
                Vector3 screenPos = new Vector3(x, y, distanceToWorldPlane);
                Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
                worldPos.z = 0f;

                float minDistance = float.MaxValue;
                int closestIndex = 0;

                for (int i = 0; i < gravityPoints.Count; i++)
                {
                    Vector2 gpPos = new Vector2(gravityPoints[i].x, gravityPoints[i].y);
                    float distance = Vector2.Distance(worldPos, gpPos)/gravityPoints[i].fieldSize;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestIndex = i;
                    }
                }

                if (colorMap.TryGetValue(closestIndex, out Color color))
                {
                    Gizmos.color = color;
                    Gizmos.DrawSphere(worldPos, 0.1f);
                }
            }
        }
    }

    private void constructData()
    {
        gravityPoints.Clear();
        colorMap.Clear();

        int i = 0;
        foreach (Transform planet in planets.transform)
        {
            GravityPointController controller = planet.GetComponent<GravityPointController>();
            if (controller != null)
            {
                GravityPoint gp = new GravityPoint
                {
                    x = planet.position.x,
                    y = planet.position.y,
                    fieldSize = controller.getFieldSize()
                };

                gravityPoints.Add(gp);
                colorMap[i] = colors[i % colors.Count];
            }
            else
            {
                Debug.LogError($"No GravityPointController attached to {planet.name}");
            }
            i++;
        }
    }
}
