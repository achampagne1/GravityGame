using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Rendering;

public class VCamController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float initialZoom = 5f;
    [SerializeField] float duration = 1f;
    [SerializeField] float magnitude = 1f;
    [SerializeField] bool shake = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.zKey.isPressed)
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 30f, Time.deltaTime*4f);
        else
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, initialZoom, Time.deltaTime*4f);

        if (shake)
        {
            StartCoroutine(shakeFunc());
            shake = false;
        }
    }

    private IEnumerator shakeFunc()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            float xOffset = Random.Range(-.5f, .5f) * magnitude;
            float yOffset = Random.Range(-.5f, .5f) * magnitude;

            transform.localPosition = new Vector3(xOffset, yOffset, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void setShake(bool shake)
    {
        this.shake = shake;
    }
}
