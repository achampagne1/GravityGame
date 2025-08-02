using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainCameraChecker : MonoBehaviour
{
    public static Transform mainCameraLocation;
    private CinemachineBrain brain;
    // Start is called before the first frame update
    void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        updateCamera();
    }

    // Update is called once per frame
    void Update()
    {
        updateCamera();
    }

    private void updateCamera()
    {
        if (brain != null && brain.ActiveVirtualCamera != null)
            mainCameraLocation = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
        else
            mainCameraLocation = transform;
    }
}
