using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject teleporterObject;
    private TeleporterController teleporterController;

    void Start()
    {
        teleporterController = teleporterObject.GetComponent<TeleporterController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "SpaceMan")
            teleporterController.setTransportTrigger(true);
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "SpaceMan")
            teleporterController.setTransportTrigger(false);
    }
}
