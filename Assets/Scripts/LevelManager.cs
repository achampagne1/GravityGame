using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [SerializeField] bool playScript = false;
    [SerializeField] GameObject teleporter;
    [SerializeField] GameObject teleporter2;
    [SerializeField] GameObject player;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject uIDocument;
    [SerializeField] GameObject asteroidTrigger1;
    [SerializeField] GameObject starList;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam2;
    [SerializeField] Cinemachine.CinemachineVirtualCamera playerCam;
    private InputSystemHelper rHelper;
    private Timer eventTimer = new Timer(30f);
    int eventChoice = 0; //0 is reserved for no choice being made or reset
    // Start is called before the first frame update
    void Start()
    {
        rHelper = new InputSystemHelper(Keyboard.current.rKey);
        if (playScript)
        {
            //set player body and hand to transparent
            setPlayerOpacity(0f,player);
            StartCoroutine(gameScript());
        }
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
        yield return new WaitForSeconds(.75f);

        //shuts off the beam
        teleporterController.setTransportTrigger(false);
        //changes camera to player
        teleporterCam.Priority = 1;
        playerCam.Priority = 2;
        //start simulation of player
        playerRb.simulated = true;
        //set player back to visible
        setPlayerOpacity(1f,player);
        yield return new WaitForSeconds(1f);

        //pulls up coms and displayes general text
        UIHandler uIHandler = uIDocument.GetComponent<UIHandler>();
        uIHandler.coms(true);
        uIHandler.setBubbleText("Coms check kid, can you hear me?",26f,8f);
        yield return new WaitForSeconds(.1f); //tiny delay for loading

        //waits for player to aknowledge or timer runs out
        eventTimer.setNewTime(10f);
        yield return new WaitUntil(acknowledgeOrWait);

        if(eventChoice == 1)
            uIHandler.setBubbleText("Good. Welcome to the training course BE-7.\nGo ahead and take a look around.",33f,14f);
        else
            uIHandler.setBubbleText("I'll take that as a yes.\nAnyway, welcome to the training course BE-7.\nGo ahead and take a look around.", 35f,20f);
        yield return new WaitForSeconds(.1f);  //tiny delay for loading

        //waits for player to aknowledge
        yield return new WaitForSeconds(10f);

        uIHandler.setBubbleText("Time to use your jetpack.\nFly up to that asteroid but watch your fuel level.", 40f, 14f);
        //gets planet trigger and check if player is intersecting
        PlanetTrigger asteroidTrigger1Trigger = asteroidTrigger1.GetComponent<PlanetTrigger>(); 
        yield return new WaitUntil(() => asteroidTrigger1Trigger.checkIfOverlapping("SpaceMan"));
        Debug.Log("done");


        //everything above is for level start
        //everything below is for end of level


        //waits until the teleporter objective is completed
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
        //sets player opacity to invisible
        setPlayerOpacity(0f,player);
        //switches to teleporter2 cam
        VCamController teleporterCamController2 = teleporterCam2.GetComponent<VCamController>();
        teleporterCam2.Priority = 2;
        playerCam.Priority = 1;
        yield return new WaitForSeconds(.1f);

        //shows level complete screen
        uIHandler.showLevelEnd();
        yield return new WaitForSeconds(1.9f);

        //shuts off the beam
        teleporterController2.setTransportTrigger(false);
    }

    private bool acknowledgeOrWait()
    {
        if (!eventTimer.getIsRunning())
            eventTimer.startTimer();
        if (rHelper.wasPressedWithCooldown())
        {
            eventChoice = 1;
            return true;
        }
        else if (eventTimer.checkTimer())
        {
            eventChoice = 2;
            return true;
        }
        else
            return false;
    }

    private void setPlayerOpacity(float opacity,GameObject gameObject)
    {
        //recursivley sets all children of the player to an opacity. could be used for other things too
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color color = sr.color;
            color.a = opacity;
            sr.color = color;
        }
        foreach(Transform child in gameObject.transform)
        {
            if(gameObject.name != "Explode")
                setPlayerOpacity(opacity, child.gameObject);
        }
    }
}
