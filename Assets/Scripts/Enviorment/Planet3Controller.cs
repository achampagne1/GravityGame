using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet3Controller : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
    private float rotationAmount = 0;
    private GameObject core;
    private GameObject glow;
    // Start is called before the first frame update
    void Start()
    {
        core = transform.Find("Core").gameObject;
        glow = transform.Find("Glow").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        rotationAmount = (rotationAmount+(Time.deltaTime*rotationSpeed))%360;
        core.transform.rotation = Quaternion.Euler(0, 0, rotationAmount);
        glow.transform.rotation = Quaternion.Euler(0, 0, -rotationAmount);
        float scale = Mathf.Lerp(0.9f, 1.3f, Mathf.Sin(rotationAmount * Mathf.Deg2Rad) * 0.5f + 0.5f);
        glow.transform.localScale = new Vector3(scale, scale, scale);
    }
}
