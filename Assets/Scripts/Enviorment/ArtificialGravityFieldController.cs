using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravityFieldController : MonoBehaviour
{
    [SerializeField] private float fieldStrength = 20f;
    [SerializeField] private Vector2 direction = new Vector2(0, -1);

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
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(fieldStrength* (Vector2)(transform.rotation * direction));
        }
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (!trigger.isTrigger)
        {
            trigger.gameObject.GetComponent<ObjectController>().setGravityOverride(Vector2.zero);
        }
    }
}
