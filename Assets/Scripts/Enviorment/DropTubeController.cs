using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropTubeController : MonoBehaviour
{
    [SerializeField] bool arrowsOn = false;
    [SerializeField] bool open = false;
    [SerializeField] float delay = .1f;
    [SerializeField] float fadeSpeed = .5f;
    [SerializeField] GameObject arrowHolder;
    private float previousFadeSpeed = 0.0f;
    private bool doneLatch = false;
    private bool arrowsChangingState = false;
    private bool stateLatch = false;
    private List<ArrowController> arrowControllerList = new List<ArrowController>();
    // Start is called before the first frame update
    void Start()
    {
        previousFadeSpeed = fadeSpeed;
        foreach(Transform child in arrowHolder.transform)
        {
            arrowControllerList.Add(child.gameObject.GetComponent<ArrowController>());
        }
        modifyFadeSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (arrowsOn&&stateLatch!=arrowsOn)
        {
            arrowFunc(true);
        }
        else if (!arrowsOn&& stateLatch != arrowsOn)
        {
            arrowFunc(false);
        }
        stateLatch = arrowsOn;
        if (previousFadeSpeed != fadeSpeed)
        {
            modifyFadeSpeed();
            previousFadeSpeed = fadeSpeed;
        }
    }
    private void arrowFunc(bool state)
    {
        float offset = 0.0f;
        for (int i = arrowControllerList.Count-1; i >=0; i--)
        {
            arrowControllerList[i].setStartValue(offset);
            arrowControllerList[i].setState(state);
            offset += delay;
        }
    }

    private void modifyFadeSpeed()
    {
        for(int i = 0; i < arrowControllerList.Count; i++)
        {
            arrowControllerList[i].setFadeSpeed(fadeSpeed);
        }
    }
}
