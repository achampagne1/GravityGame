using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private bool runGenerateNavigationMesh = false;
    [SerializeField] private Vector2 meshCenter = Vector2.zero;
    [SerializeField] private Vector2 navigationMeshSize = new Vector2(10, 10);
    [SerializeField] private float navigationMeshResolution = 1f;
    private Coroutine navigationMeshGenerationCoroutine;

    public void FixedUpdate()
    {
        if (runGenerateNavigationMesh && runGenerateNavigationMesh == null)
        {
            navigationMeshGenerationCoroutine = StartCoroutine(generateNavigationMesh());
            runGenerateNavigationMesh = false;
        }
    }

    private IEnumerator generateNavigationMesh()
    {
        float x = -navigationMeshSize.x / 2f;
        float y = -navigationMeshSize.y / 2f;
        while (x < navigationMeshSize.x / 2)
        {
            while (y < navigationMeshSize.y / 2)
            {
                Vector2 position = new Vector2(x, y) + meshCenter;
                Collider2D[] colliders = Physics2D.OverlapBoxAll(
                    position,
                    new Vector2(4.5f, 4.5f),
                    0f,
                    obstacleLayers
                );

                foreach (Collider2D collider in colliders)
                {
                    Debug.Log(collider.gameObject.name);
                }
                y += navigationMeshResolution;
            }
            x += navigationMeshResolution;
        }
        yield return null;
    }

    public static List<Vector2> findPath(Transform from,Transform to,NavigationProfile navigationProfile)
    {
        return new List<Vector2>();
    }
}
