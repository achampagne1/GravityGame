using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeController : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    private Transform center;
    public void trigger(Vector2 center)
    {
        Transform chunks = transform.Find("Chunks");
        foreach (Transform chunk in chunks)
            chunk.gameObject.GetComponent<FragmentController>().setExplode(center);
        Vector3 direction = (transform.position - new Vector3(center.x,center.y,0f)).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        var shape = particleSystem.shape;
        shape.rotation = rotation.eulerAngles;
        particleSystem.Play(); 
    }

}
