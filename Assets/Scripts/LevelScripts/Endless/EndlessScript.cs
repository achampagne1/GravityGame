using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 ****Level mangment rules****
 *all wait until events must be wrapped in as an objective. they dont need to have text ascociated but must be wrapped
 *the only exception is aknowledge or wait
 **/
public class EndlessScript : MonoBehaviour
{
    [SerializeField] bool playScript = false;
    [SerializeField] GameObject player;
    [SerializeField] GameObject uIDocument;
    [SerializeField] GameObject enemies;
    [SerializeField] float[] playArea = { 50, 50 }; //generic play area 
    [SerializeField] GameObject wayPoint;
    [SerializeField] GameObject wayPoints;
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
        spaceManController = player.GetComponent<SpaceManController>();
        if (playScript)
        {
            yield return null;
            StartCoroutine(gameScript());
            StartCoroutine(failConditions());
        }
        yield return null;
    }

    private IEnumerator gameScript()
    {
        //KEEP UP WITH THE COMMENTS!!!
        //Each action should have a comment

        //pulls up coms and displayes general text
        yield return new WaitForSeconds(1f); //tiny delay for loading
        uIHandler.setBubbleText("Sorry Kid, we cant rescue you. You're on your own. All we can do is drop in fuel and med kits");
        yield return new WaitForSeconds(.1f); //tiny delay for loading

        //waits for player to aknowledge or timer runs out
        eventTimer.setNewTime(10f);
        eventTimer.resetTimer();
        yield return new WaitUntil(acknowledgeOrWait);
    }

    private IEnumerator failConditions()
    {
        while (true)
        {
            //for all the failure conditions
            if (spaceManController.getDead() ||
                (Mathf.Abs(player.transform.position.x) > playArea[0] || Mathf.Abs(player.transform.position.y) > playArea[1]) ||
                spaceManController.getCurrentFuel() == 0)
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

        foreach (Vector2 wayPointLocation in wayPointLocations)
        {
            GameObject arrow = Instantiate(wayPoint, Vector3.zero, Quaternion.identity);
            arrow.transform.SetParent(wayPoints.transform);
            WayPointController wayPointController = arrow.GetComponent<WayPointController>();
            wayPointController.setPlayerTransform(player.transform);
            wayPointController.setPointLocation(wayPointLocation);
        }
    }
}