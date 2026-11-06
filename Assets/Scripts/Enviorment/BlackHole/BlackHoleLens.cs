using UnityEngine;

public class BlackHoleLens : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera lensCamera;
    [SerializeField] private int textureSize = 512;
    [SerializeField] private float viewScale = 4f;

    private static readonly int LensTexId = Shader.PropertyToID("_LensTex");
    private static readonly int LensCenterId = Shader.PropertyToID("_LensCenter");
    private static readonly int LensHalfViewId = Shader.PropertyToID("_LensHalfView");

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private RenderTexture rt;

    private void OnEnable()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        if (mainCamera == null) mainCamera = Camera.main;

        rt = new RenderTexture(textureSize, textureSize, 24)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        rt.Create();

        lensCamera.targetTexture = rt;
        lensCamera.depth = mainCamera.depth - 1f;
    }

    private void OnDisable()
    {
        if (lensCamera != null) lensCamera.targetTexture = null;
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }
    }

    private void LateUpdate()
    {
        Bounds b = rend.bounds;
        float halfView = b.extents.x * viewScale;

        lensCamera.transform.position = new Vector3(b.center.x, b.center.y, mainCamera.transform.position.z);
        lensCamera.orthographicSize = halfView;

        lensCamera.enabled = rend.isVisible;

        rend.GetPropertyBlock(mpb);
        mpb.SetTexture(LensTexId, rt);
        mpb.SetVector(LensCenterId, new Vector4(b.center.x, b.center.y, 0f, 0f));
        mpb.SetFloat(LensHalfViewId, halfView);
        rend.SetPropertyBlock(mpb);
    }
}