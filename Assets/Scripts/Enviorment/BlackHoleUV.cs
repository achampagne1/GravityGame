using UnityEngine;

public class BlackHoleUV : MonoBehaviour
{
    [SerializeField] private Transform blackHole;
    [SerializeField] private Material blackHoleMaterial;
    [SerializeField] private Camera targetCamera;

    private void LateUpdate()
    {
        if (blackHole == null || blackHoleMaterial == null)
            return;

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
