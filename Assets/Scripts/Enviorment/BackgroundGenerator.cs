using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BackgroundGenerator : MonoBehaviour
{
    private Camera renderCamera;
    private SpriteRenderer backgroundRenderer;

    void Start()
    {
        renderCamera = Camera.main;
        backgroundRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (renderCamera == null)
            renderCamera = Camera.main;

        if (renderCamera == null)
            return;

        Vector3 center = renderCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, renderCamera.nearClipPlane + 0.01f));
        transform.position = new Vector3(center.x, center.y, center.z);
    }
}
