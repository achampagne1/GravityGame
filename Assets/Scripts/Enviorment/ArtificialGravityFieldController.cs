using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravityFieldController : MonoBehaviour
{
    public float fieldStrength = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(!trigger.isTrigger)
        {
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(fieldStrength* (Vector2)(transform.rotation * Vector2.down));
        }
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (!trigger.isTrigger)
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(Vector2.zero);
    }
}
