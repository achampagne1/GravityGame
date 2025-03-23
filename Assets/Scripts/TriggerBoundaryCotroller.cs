using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoundaryCotroller : MonoBehaviour
{
    //object creation
    private GameObject characterHand;

    // Start is called before the first frame update
    void Start()
    {
        characterHand = GameObject.Find("SpaceManHand");
        Physics2D.IgnoreLayerCollision(9, 14, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(trigger.gameObject.name == "Gun" && !trigger.gameObject.GetComponent<GunController>().getParented())//will need to change to item controller once parenting is moved to item
        {
            trigger.gameObject.GetComponent<GunController>().setParent(characterHand);
            Debug.Log("grab");
        }
    }
}
