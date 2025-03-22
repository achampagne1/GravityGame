using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryCotroller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 14, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
