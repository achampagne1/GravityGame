using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject teleporter;
    [SerializeField] GameObject player;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam;
    [SerializeField] Cinemachine.CinemachineVirtualCamera playerCam;
    // Start is called before the first frame update
    void Start()
    {

        //start the script
        StartCoroutine(gameScript());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator gameScript()
    {
        //KEEP UP WITH THE COMMENTS!!!
        //Each action should have a comment

        //activate teleporter first
        TeleporterController teleporterController = teleporter.GetComponent<TeleporterController>();
        teleporterController.toggleStateFunc();
        yield return new WaitForSeconds(2f);

        //turns on the transport beam
        teleporterController.setTransportTrigger(true);
        yield return new WaitForSeconds(.2f);

        //puts player on pad(spawnPoint)
        player.transform.position = spawnPoint.transform.position;
        //changes camera to player
        teleporterCam.Priority = 1;
        playerCam.Priority = 2;
        //start simulation of player
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.simulated = true;
        //add camera shake
        yield return new WaitForSeconds(2f);

        //shuts off the beam
        teleporterController.setTransportTrigger(false);
        yield return new WaitForSeconds(2f);
    }
}
