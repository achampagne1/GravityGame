using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/**
 ****Level mangment rules****
 *all wait until events must be wrapped in as an objective. they dont need to have text ascociated but must be wrapped
 *the only exception is aknowledge or wait
 **/
public class LevelManager : MonoBehaviour
{
    [SerializeField] bool playScript = false;
    [SerializeField] GameObject teleporter;
    [SerializeField] GameObject teleporter2;
    [SerializeField] GameObject player;
    [SerializeField] GameObject greenTeamOriginal;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject uIDocument;
    [SerializeField] GameObject starList;
    [SerializeField] GameObject enemies;
    [SerializeField] GameObject wayPoint;
    [SerializeField] GameObject wayPoints;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam;
    [SerializeField] Cinemachine.CinemachineVirtualCamera teleporterCam2;
    [SerializeField] Cinemachine.CinemachineVirtualCamera playerCam;
    [SerializeField] float[] playArea = { 50, 50 }; //generic play area 
    private SpaceManController spaceManController;
    private Timer eventTimer = new Timer(30f);
    private UIHandler uIHandler;
    private ObjectiveLoader objectiveLoader;
    private Dictionary<string, Objective> objectives;
    private Objective currentObjective;
    private bool objectivesLoading = true;
    int eventChoice = 0; //0 is reserved for no choice being made or reset
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(initialCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator initialCoroutine()
    {
        uIHandler = uIDocument.GetComponent<UIHandler>();
        objectiveLoader = new ObjectiveLoader();
        spaceManController = player.GetComponent<SpaceManController>();
        if (playScript)
        {
            teleporterCam.Priority = 2; //just so the camera is set up at start
            playerCam.Priority = 1;

            StartCoroutine(loadObjectives());
            yield return new WaitUntil(() => !objectivesLoading); //waits for the loading of the objectives to complete

            //set player body and hand to transparent
            setPlayerOpacity(0f, player);
            StartCoroutine(gameScript());
            StartCoroutine(failConditions());
        }
    }

    private IEnumerator gameScript()
    {
        //KEEP UP WITH THE COMMENTS!!!
        //Each action should have a comment

        //Sets initial objective
        changeObjective("nocurrentobjective");
        //sets teleporter cam to main cam
        VCamController teleporterCamController = teleporterCam.GetComponent<VCamController>();
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
        uIHandler.setBubbleText("Coms check kid, can you hear me?");
        yield return new WaitForSeconds(.1f); //tiny delay for loading

        //waits for player to aknowledge or timer runs out
        eventTimer.setNewTime(10f);
        eventTimer.resetTimer();
        yield return new WaitUntil(acknowledgeOrWait);

        if(eventChoice == 1)
            uIHandler.setBubbleText("Good. Welcome to the training course BE-7.\nGo ahead and take a look around.");
        else
            uIHandler.setBubbleText("I'll take that as a yes.\nAnyway, welcome to the training course BE-7.\nGo ahead and take a look around.");
        yield return new WaitForSeconds(.1f);  //tiny delay for loading

        //waits for player to aknowledge
        yield return new WaitForSeconds(10f); //do this better

        //displayes orders
        uIHandler.setBubbleText("Time to use your jetpack.\nFly up to that asteroid but watch your fuel level.");
        //changes objective
        changeObjective("flytoasteroid");
        //gets planet trigger and check if player is intersecting
        yield return new WaitUntil(currentObjective.completionCondition);

        //sets objective as get your gun and waits until it is completed
        uIHandler.setBubbleText("Good work. There is a gun in the space station.\nGo ahead and pick it up. I added it as an objective.");
        //changes objective
        changeObjective("getyourgun");
        yield return new WaitUntil(currentObjective.completionCondition);
        //kill all bugs 
        uIHandler.setBubbleText("Now for target practice. You see those bugs?\nTake em out!");
        //changes objective
        changeObjective("killallbugs");
        yield return new WaitUntil(currentObjective.completionCondition);

        //sets next objective to get to teleporter
        //changes objective
        changeObjective("gettoteleporter");

        //get to teleporter
        uIHandler.setBubbleText("Thats about it for training today.\nStart making your way to the teleporter.");
        //activate teleporter2
        TeleporterController teleporterController2 = teleporter2.GetComponent<TeleporterController>();
        teleporterController2.toggleStateFunc();
        //waits for player to intersect with enemy trigger
        yield return new WaitUntil(currentObjective.completionCondition);


        //generals orders
        teleporterController2.toggleStateFunc();
        uIHandler.setBubbleText("Uh oh, hang on kid! Looks like you've got company!");
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
        uIHandler.setBubbleText("Thats the Green Team! How'd they find this place?!\nWe can't beam you up while they're here.");
        changeObjective("defeatthegreenteam");
        yield return new WaitUntil(currentObjective.completionCondition);

        //green team defeated
        uIHandler.setBubbleText("WOOOO you took care of those guys!\nNow we can get you. Get back to the teleporter.");
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
        uIHandler.showLevelEnd(true);
        yield return new WaitForSeconds(1.9f);

        //shuts off the beam
        teleporterController2.setTransportTrigger(false);
        yield return new WaitForSeconds(2f);

        //returns to main menu
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator failConditions()
    {
        while (true)
        {
            //for all the failure conditions
            if(spaceManController.getDead()|| 
                (Mathf.Abs(player.transform.position.x) > playArea[0] || Mathf.Abs(player.transform.position.y) > playArea[1])||
                spaceManController.getCurrentFuel()==0)
            {
                uIHandler.showLevelEnd(false);
                yield return new WaitForSeconds(5f);
                SceneManager.LoadScene("MainMenu");
            }
            yield return new WaitForSeconds(.2f);
        }
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

    private void changeObjective(string name)
    {
        currentObjective = objectives[name];
        uIHandler.setCurrentObjective(currentObjective);
        wayPointAbstraction();
    }

    private IEnumerator loadObjectives()
    {
        var initializeTask = objectiveLoader.initializeObjectives(); //calls the initialize function that returns a task
        while (!initializeTask.IsCompleted) //waits for task to complete
            yield return null;
        objectives = objectiveLoader.getObjectives();
        objectivesLoading = false;
    }

    private void wayPointAbstraction()
    {
        foreach (Transform child in wayPoints.transform) //clears all previous waypoints
            GameObject.Destroy(child.gameObject);

        if (currentObjective.wayPointLocations == null)
            return;

        List<Vector2> wayPointLocations = currentObjective.wayPointLocations();
        
        foreach(Vector2 wayPointLocation in wayPointLocations)
        {
            GameObject arrow = Instantiate(wayPoint, Vector3.zero, Quaternion.identity);
            arrow.transform.SetParent(wayPoints.transform);
            WayPointController wayPointController = arrow.GetComponent<WayPointController>();
            wayPointController.setPlayerTransform(player.transform);
            wayPointController.setPointLocation(wayPointLocation);
        }
    }

    private void setPlayerOpacity(float opacity,GameObject gameObject)
    {
        //recursivley sets all children of the player to an opacity. could be used for other things too
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            HelperFunctions.changeOpacity(sr, opacity);
        }
        foreach(Transform child in gameObject.transform)
        {
            if(gameObject.name != "Explode")
                setPlayerOpacity(opacity, child.gameObject);
        }
    }


}
