using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class VCamController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float initialZoom = 5f;
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
    }
}
