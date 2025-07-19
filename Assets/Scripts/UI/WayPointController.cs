using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointController : MonoBehaviour
{
    [SerializeField] Transform player;
    private Vector2 objective = new Vector2(-8f, -8f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] corners = new Vector3[4];

        float z = Camera.main.nearClipPlane;
        corners[0] = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z));
        corners[1] = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, z));
        corners[2] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, z));
        corners[3] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, z));

        Vector2 intersectionPoint = new Vector2();
        for(int i = 0; i < 4; i++)
        {
            Vector2 intersectionPointLocal;
            bool intersects = GetLineSegmentIntersection(corners[i], corners[(i+1)%4], (Vector2)player.position, objective, out intersectionPointLocal);
            Debug.Log(corners[i]+" "+ corners[(i + 1) % 4]+" "+ (Vector2)transform.position+" "+objective);
            if (intersects)
            {
                intersectionPoint = intersectionPointLocal;
                break;
            }
        }
        transform.position = new Vector3(intersectionPoint[0], intersectionPoint[1],player.position.z);
        float angle = Mathf.Atan2(objective.y- transform.position.y,objective.x-transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle-90);
    }

    private bool GetLineSegmentIntersection(
    Vector2 A, Vector2 B, Vector2 C, Vector2 D, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 r = B - A;
        Vector2 s = D - C;

        float rxs = Cross(r, s);
        Vector2 AC = C - A;
        float ACxr = Cross(AC, r);

        if (Mathf.Approximately(rxs, 0f))
        {
            // Lines are parallel or collinear
            return false;
        }

        float t = Cross(AC, s) / rxs;
        float u = ACxr / rxs;

        // Check if intersection point is on both line segments
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = A + t * r;
            return true;
        }

        return false;
    }

    // Helper to calculate the 2D cross product (scalar)
    private float Cross(Vector2 v, Vector2 w)
    {
        return v.x * w.y - v.y * w.x;
    }
}

