using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeController : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] GameObject center;
    public void trigger()
    {
        foreach(Transform fragment in transform)
        {
            try //the try catch is needed because the first child of the controller isjust the center point
            {
                fragment.gameObject.GetComponent<FragmentController>().setExplode(true);
            }
            catch
            {
                int ham = 1;
            }
        }
        Vector3 direction = (transform.position - center.transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        var shape = particleSystem.shape;
        shape.rotation = rotation.eulerAngles;
        particleSystem.Play(); 
    }


}
