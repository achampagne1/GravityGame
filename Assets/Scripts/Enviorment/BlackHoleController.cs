using UnityEngine;
using UnityEngine.Device;

public class BlackHoleController : MonoBehaviour
{
    [SerializeField] private GameObject distortion;
    [SerializeField] private Transform eventHorizonTransform;
    [SerializeField] private float eventHorizonRadius = .1f;
    private Material distortionMaterial;

    void Start()
    {
        distortionMaterial = distortion.GetComponent<Renderer>().material;
    }

    
    void FixedUpdate()
    {
        eventHorizonTransform.localScale = new Vector3(
            eventHorizonRadius * 10f,
            eventHorizonRadius * 10f,
            eventHorizonRadius * 10f
        );
        distortionMaterial.SetFloat(
            "_HoleRadius",
            eventHorizonRadius
        );
    }
}
