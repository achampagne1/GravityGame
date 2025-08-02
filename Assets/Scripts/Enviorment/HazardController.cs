using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour, IDamager
{
    [SerializeField] float damageVariable = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float damage(GameObject gameobject)
    {
        return damageVariable;
    }

    public float getDamage()
    {
        return damageVariable;
    }
}
