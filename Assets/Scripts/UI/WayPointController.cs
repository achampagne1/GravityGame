using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float arrowPadding = 0.5f;
    private SpriteRenderer sr;
    private Vector2 pointLocation;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
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
        bool intersects = false;
        for(int i = 0; i < 4; i++)
        {
            Vector2 intersectionPointLocal;
            bool intersectsLocal = GetLineSegmentIntersection(corners[i], corners[(i+1)%4], (Vector2)player.position, pointLocation, out intersectionPointLocal);
            if (intersectsLocal)
            {
                intersectionPoint = intersectionPointLocal;
                intersects = true;
                HelperFunctions.changeOpacity(sr, 1);
                break;
            }
        }

        if (!intersects)
        {
            HelperFunctions.changeOpacity(sr, 0);
        }

        Vector2 direction = ((Vector2)player.position - intersectionPoint).normalized;
        intersectionPoint += direction * arrowPadding;

        transform.position = new Vector3(intersectionPoint.x, intersectionPoint.y, player.position.z);

        float angle = Mathf.Atan2(pointLocation.y- transform.position.y,pointLocation.x-transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle-90);
    }

    //written by chat gpt
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

    public void setPointLocation(Vector2 pointLocation)
    {
        this.pointLocation = pointLocation;
    }

    public void setPlayerTransform(Transform player)
    {
        this.player = player;
    }
}

