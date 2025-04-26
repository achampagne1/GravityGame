using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] bool playScript = false;
    [SerializeField] GameObject teleporter;
    [SerializeField] GameObject teleporter2;
    [SerializeField] GameObject player;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject uIDocument;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam2;
    [SerializeField] Cinemachine.CinemachineVirtualCamera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        //start the script
        if (playScript)
            StartCoroutine(gameScript());
        //gotta swap the camera priorities too
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator gameScript()
    {
        //KEEP UP WITH THE COMMENTS!!!
        //Each action should have a comment

        //sets teleporter cam to main cam
        VCamController teleporterCamController = teleporterCam.GetComponent<VCamController>();
        teleporterCam.Priority = 2;
        playerCam.Priority = 1;
        //turns off player simulation
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.simulated = false;
        //activate teleporter
        TeleporterController teleporterController = teleporter.GetComponent<TeleporterController>();
        teleporterController.toggleStateFunc();
        yield return new WaitForSeconds(1f);

        //turns on the transport beam
        teleporterController.setTransportTrigger(true);
        //shakes camera
        //teleporterCamController.setShake(true);
        yield return new WaitForSeconds(.2f);

        //puts player on pad(spawnPoint)
        player.transform.position = spawnPoint.transform.position;
        //turns off teleporter
        teleporterController.toggleStateFunc();
        yield return new WaitForSeconds(2f);

        //shuts off the beam
        teleporterController.setTransportTrigger(false);
        //changes camera to player
        teleporterCam.Priority = 1;
        playerCam.Priority = 2;
        //start simulation of player
        playerRb.simulated = true;

        //everything above is for level start
        //everything below is for end of level

        //waits until the teleporter objective is completed
        UIHandler uIHandler = uIDocument.GetComponent<UIHandler>();
        yield return new WaitUntil(() => uIHandler.getCurrentObjective().name == "gettoteleporter");

        //activate teleporter2
        TeleporterController teleporterController2 = teleporter2.GetComponent<TeleporterController>();
        teleporterController2.toggleStateFunc();
        //waits until player is on the pad
        yield return new WaitUntil(() => teleporterController2.getPlayerOnPad());

        //waits an aditional half second for suspense
        yield return new WaitForSeconds(.5f);

        //turns on transport beam
        teleporterController2.setTransportTrigger(true);
        //moves player offScreen
        player.transform.position = new Vector3(-100f, -100f, 0f);
        //switches to teleporter2 cam
        VCamController teleporterCamController2 = teleporterCam2.GetComponent<VCamController>();
        teleporterCam2.Priority = 2;
        playerCam.Priority = 1;
        //shows level complete screen
        uIHandler.showLevelEnd();
        yield return new WaitForSeconds(2f);

        //shuts off the beam
        teleporterController2.setTransportTrigger(false);
    }
}
