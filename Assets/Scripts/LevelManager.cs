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
    [SerializeField] GameObject greenTeamOriginal;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject uIDocument;
    [SerializeField] GameObject asteroidTrigger1;
    [SerializeField] GameObject starList;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject enemySpawnTrigger;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam2;
    [SerializeField] Cinemachine.CinemachineVirtualCamera playerCam;
    private Timer eventTimer = new Timer(30f);
    private UIHandler uIHandler;
    private ObjectiveWrapper objectiveWrapper;
    private Objective currentObjective;
    int eventChoice = 0; //0 is reserved for no choice being made or reset
    // Start is called before the first frame update
    void Start()
    {
        uIHandler = uIDocument.GetComponent<UIHandler>();
        objectiveWrapper = new ObjectiveWrapper();
        StartCoroutine(objectivesStart());
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
        uIHandler.setBubbleText("Coms check kid, can you hear me?",26f,8f);
        yield return new WaitForSeconds(.1f); //tiny delay for loading

        //waits for player to aknowledge or timer runs out
        eventTimer.setNewTime(10f);
        eventTimer.resetTimer();
        yield return new WaitUntil(acknowledgeOrWait);

        if(eventChoice == 1)
            uIHandler.setBubbleText("Good. Welcome to the training course BE-7.\nGo ahead and take a look around.",33f,14f);
        else
            uIHandler.setBubbleText("I'll take that as a yes.\nAnyway, welcome to the training course BE-7.\nGo ahead and take a look around.", 35f,20f);
        yield return new WaitForSeconds(.1f);  //tiny delay for loading

        //waits for player to aknowledge
        yield return new WaitForSeconds(10f); //do this better

        //displayes orders
        uIHandler.setBubbleText("Time to use your jetpack.\nFly up to that asteroid but watch your fuel level.", 40f, 14f);
        //gets planet trigger and check if player is intersecting
        PlanetTrigger asteroidTrigger1Trigger = asteroidTrigger1.GetComponent<PlanetTrigger>(); 
        yield return new WaitUntil(() => asteroidTrigger1Trigger.checkIfOverlapping("SpaceMan"));

        //sets objective as get your gun and waits until it is completed
        uIHandler.setBubbleText("Good work. There is a gun in the space station.\nGo ahead and pick it up. I added it as an objective.", 42f, 14f);
        yield return objectivesStuff(true);
        //kill all bugs 
        uIHandler.setBubbleText("Now for target practice. You see those bugs?\nTake em out!", 35f, 14f);
        yield return objectivesStuff(true);

        //sets next objective to get to teleporter
        yield return objectivesStuff(false);

        //get to teleporter
        uIHandler.setBubbleText("Thats about it for training today.\nStart making your way to the teleporter.", 32f, 14f);
        //activate teleporter2
        TeleporterController teleporterController2 = teleporter2.GetComponent<TeleporterController>();
        teleporterController2.toggleStateFunc();
        //waits for player to intersect with enemy trigger
        PlanetTrigger trigger = enemySpawnTrigger.GetComponent<PlanetTrigger>();
        yield return new WaitUntil(() => trigger.checkIfOverlapping("SpaceMan"));


        //generals orders
        uIHandler.setBubbleText("Uh oh, hang on kid! Looks like you've got company!", 40f, 8f);
        //spawns green team
        GameObject one = Instantiate(greenTeamOriginal, new Vector3(-36f,-6f,0f), Quaternion.identity);
        one.tag = "SpaceZombie";
        one.transform.SetParent(enemies.transform);
        one.GetComponent<Rigidbody2D>().simulated = true;
        GameObject two = Instantiate(greenTeamOriginal, new Vector3(-53f, -25f, 0f), Quaternion.identity);
        two.GetComponent<Rigidbody2D>().simulated = true;
        two.tag = "SpaceZombie";
        two.transform.SetParent(enemies.transform);
        GameObject three = Instantiate(greenTeamOriginal, new Vector3(-36f, -36f, 0f), Quaternion.identity);
        three.tag = "SpaceZombie";
        three.GetComponent<Rigidbody2D>().simulated = true;
        three.transform.SetParent(enemies.transform);
        GameObject four = Instantiate(greenTeamOriginal, new Vector3(-22f, -22f, 0f), Quaternion.identity);
        four.tag = "SpaceZombie";
        four.GetComponent<Rigidbody2D>().simulated = true;
        four.transform.SetParent(enemies.transform);
        //waits for player to aknowledge or timer runs out
        eventTimer.setNewTime(10f);
        yield return new WaitUntil(acknowledgeOrWait);
        uIHandler.setBubbleText("Thats the Green Team! How'd they find this place?!\nWe can't beam you up while they're here.", 40f, 14f);
        teleporterController2.toggleStateFunc();
        yield return objectivesStuff(true);

        //green team defeated
        uIHandler.setBubbleText("WOOOO you took care of those guys!\nNow we can get you. Get back to the teleporter.", 37f, 14f);
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
        if (uIHandler.getAcknowledgeComs())
        {
            eventChoice = 1;
            eventTimer.resetTimer();
            return true;
        }
        else if (eventTimer.checkTimer())
        {
            eventChoice = 2;
            eventTimer.resetTimer();
            return true;
        }
        else
            return false;
    }

    //this is to abstract all the tiny stuff needed for objectives
    private IEnumerator objectivesStuff(bool wait)
    {
        currentObjective = objectiveWrapper.getNextObjective();
        Debug.Log(currentObjective.name);
        uIHandler.setCurrentObjective(currentObjective);
        if(wait)
            yield return new WaitUntil(() => currentObjective.completionCondition());
        else
            yield return null;
    }

    private IEnumerator objectivesStart()
    {
        var initializeTask = objectiveWrapper.initializeObjectives();
        while (!initializeTask.IsCompleted)
            yield return null;
        currentObjective = objectiveWrapper.getNextObjective();
        uIHandler.setCurrentObjective(currentObjective);
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
