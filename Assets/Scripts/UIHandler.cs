using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    //game variables
    public float currentHealth = 1f;
    public float movePercent = 0f;
    private float fadeCounter = 180f;
    private float parentTop = 0f;
    private bool escapeClicked = false;
    private bool eClicked = false;
    private bool coroutineRunning = false;
    public float speed = .05f;
    public float shiftNum = 4f;
    
    //object creation
    public static UIHandler instance { get; private set; }

    private VisualElement fullBar;
    private VisualElement fullFuelBar;
    private VisualElement warningBar;
    private VisualElement pauseMenu;
    private VisualElement darken;
    private VisualElement overlayContainer;
    private VisualElement pauseContainer;
    private VisualElement objectiveContainer;
    private VisualElement offScreenObjectivesContainer;
    private VisualElement killEnemyObjective;
    private VisualElement exitGameButton;
    private VisualElement[] overlayArray = new VisualElement[25];
    private IEnumerator<VisualElement> objectives;

    private InputSystemHelper escapeKey;
    private InputSystemHelper eKey;

    private Coroutine moveScanLinesCoroutine;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        fullBar = uiDocument.rootVisualElement.Q<VisualElement>("healthBar");
        fullFuelBar = uiDocument.rootVisualElement.Q<VisualElement>("fuelBar");
        warningBar = uiDocument.rootVisualElement.Q<VisualElement>("warningBar");
        pauseMenu = uiDocument.rootVisualElement.Q<VisualElement>("pause");
        pauseContainer = uiDocument.rootVisualElement.Q<VisualElement>("pauseContainer");
        darken = uiDocument.rootVisualElement.Q<VisualElement>("darken");
        exitGameButton = uiDocument.rootVisualElement.Q<Button>("exitGameButton");
        objectiveContainer = uiDocument.rootVisualElement.Q<VisualElement>("text");
        killEnemyObjective = uiDocument.rootVisualElement.Q<VisualElement>("killEnemy");
        overlayContainer = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainer");
        offScreenObjectivesContainer = uiDocument.rootVisualElement.Q<VisualElement>("offScreenObjectives");
        overlayArray = overlayContainer.Query<VisualElement>("overlay").ToList().ToArray();
        objectives = offScreenObjectivesContainer.Children().GetEnumerator(); 
        warningBar.style.opacity = 0f;
        pauseContainer.style.opacity = 0f;
        objectiveContainer.style.opacity = 0f;
        pauseMenu.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        escapeKey = new InputSystemHelper(Keyboard.current.escapeKey);
        eKey = new InputSystemHelper(Keyboard.current.eKey);
        pauseMenu.style.top = Length.Percent(110);

        exitGameButton.RegisterCallback<ClickEvent>(exitGame); //gotta figure this out
        objectives.MoveNext();
        objectiveContainer.style.backgroundImage = objectives.Current.resolvedStyle.backgroundImage;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= .3f)
        {
            warningBarFunction();
        }
        if (escapeKey.wasPressedWithCooldown()&&!eClicked)
        {
            escapeClicked = !escapeClicked;
            if (escapeClicked)
            {
                //Time.timeScale = 0; //this pauses the scan lines too for some reason
                pauseMenu.style.top = Length.Percent(0);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.7f);
                moveScanLinesCoroutine = StartCoroutine(shiftOverlayRoutine());
                pauseContainer.style.opacity = 1;
            }
            else
            {
                Time.timeScale = 1;
                pauseMenu.style.top = Length.Percent(110);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.0f);
                StopCoroutine(moveScanLinesCoroutine);
                pauseContainer.style.opacity = 0;
            }
        }
        if (eKey.wasPressedWithCooldown()&&!escapeClicked)
        {
            eClicked = !eClicked;
            if (eClicked)
            {
                pauseMenu.style.top = Length.Percent(movePercent);
                moveScanLinesCoroutine = StartCoroutine(shiftOverlayRoutine());
                objectiveContainer.style.opacity = 1;
            }
            else
            {
                pauseMenu.style.top = Length.Percent(110);
                StopCoroutine(moveScanLinesCoroutine);
                objectiveContainer.style.opacity = 0;
            }
        }
    }

    private IEnumerator shiftOverlayRoutine()
    {
        while (true)
        {

            for (int i = 0; i < 25; i++)
            {
                parentTop = overlayContainer.resolvedStyle.height;
                float topInPixels = overlayArray[i].resolvedStyle.top;
                float topPercent = (topInPixels / parentTop) * 100f;
                float shiftPixels = (shiftNum / 100f) * parentTop;
                if (topPercent > 85)
                    overlayArray[i].style.top = new Length(0, LengthUnit.Pixel);
                else
                    overlayArray[i].style.top = new Length(topInPixels + shiftPixels, LengthUnit.Pixel);
            }
            yield return new WaitForSecondsRealtime(speed);// Adjust delay for smoother shifting
        }
    }

    private void exitGame(ClickEvent evt)
    {
        Debug.Log("here");
        Application.Quit();
    }


    public void setHealthValue(float health)
    {
        currentHealth = health / 10f;
        fullBar.style.width = Length.Percent(currentHealth * 100.0f);
    }

    public void setFuelValue(float fuelLevel)
    {
        fullFuelBar.style.width = Length.Percent(fuelLevel);
    }

    private void warningBarFunction()
    {
        if (currentHealth == .3f)
            fadeCounter -= 2f;
        else if (currentHealth == .2f)
            fadeCounter -= 4f;
        else if (currentHealth == .1f)
            fadeCounter -= 8f;

        warningBar.style.opacity = 1f - Mathf.Sin(fadeCounter * Mathf.Deg2Rad);
        warningBar.style.width = Length.Percent(currentHealth * 100.0f);
        if (fadeCounter <= 0)
            fadeCounter = 180;
    }
}