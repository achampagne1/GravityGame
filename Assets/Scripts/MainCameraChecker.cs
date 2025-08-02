using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainCameraChecker : MonoBehaviour
{
    public static Transform mainCameraLocation;
    private CinemachineBrain brain;
    // Start is called before the first frame update
    void Start()
    {
        //brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (brain != null && brain.ActiveVirtualCamera != null)
            mainCameraLocation = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
        else
            mainCameraLocation = transform;*/
    }
}
