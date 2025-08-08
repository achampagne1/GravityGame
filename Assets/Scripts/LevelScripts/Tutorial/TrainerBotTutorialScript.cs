using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerBotTutorialScript : MonoBehaviour
{
    //game variables
    [SerializeField] float hoverDuration = 1f;
    [SerializeField] float pauseDuration = 1f;

    //objects
    [SerializeField] GameObject trainerBot;
    private TrainerController trainerBotController;

    // Start is called before the first frame update
    void Start()
    {
        trainerBotController = trainerBot.GetComponent<TrainerController>();
        StartCoroutine(hoverToPlanet());
    }

    private IEnumerator hoverToPlanet()
    {
        while (true)
        {
            trainerBotController.setJump(true);
            yield return new WaitForSeconds(hoverDuration);
            trainerBotController.setJump(false);
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
