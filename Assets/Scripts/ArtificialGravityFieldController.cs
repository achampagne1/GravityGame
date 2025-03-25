using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravityFieldController : MonoBehaviour
{
    public Vector2 fieldStrength = new Vector2(-20,0);

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
        trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(fieldStrength);
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(Vector2.zero);
    }
}
