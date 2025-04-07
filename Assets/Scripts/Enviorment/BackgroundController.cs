using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // Object creation
    private Transform playerBody;

    void Start()
    {
        playerBody = GameObject.Find("SpaceMan").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerBody.position;
    }
}
