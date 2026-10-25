using UnityEngine;

public class BlackHoleUV : MonoBehaviour
{
    private Transform blackHole;
    private Material blackHoleMaterial;
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        blackHole = transform.parent;
        blackHoleMaterial = GetComponent<Renderer>().material;
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        Vector3 screenPos =
            targetCamera.WorldToScreenPoint(blackHole.position);

        Vector2 screenUV = new Vector2(
            screenPos.x / Screen.width,
            screenPos.y / Screen.height
        );

        blackHoleMaterial.SetVector(
            "_ScreenUV",
            new Vector4(screenUV.x, screenUV.y, 0, 0)
        );
    }
}
