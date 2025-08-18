using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BackgroundGenerator : MonoBehaviour
{
    private Transform followPoint;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 camPos = MainCameraChecker.mainCameraLocation.position;
        transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
    }
    void Update()
    {
        Vector3 camPos = MainCameraChecker.mainCameraLocation.position;
        transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
    }
}
