using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    public float currentHealth = 1f;
    public static UIHandler instance { get; private set; }
    private float fadeCounter = 180f;
    private bool escapeClicked = false;
    private float lastEscapePress = 0f;
    private float parentTop = 0f;
    public float speed = .05f;
    public float shiftNum = 4f;
    private VisualElement fullBar;
    private VisualElement fullFuelBar;
    private VisualElement warningBar;
    private VisualElement pauseMenu;
    private VisualElement darken;
    private VisualElement overlayContainer;
    private VisualElement exitGameButton;
    private VisualElement[] overlayArray = new VisualElement[25];
    private InputSystemHelper escapeKey;
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
        darken = uiDocument.rootVisualElement.Q<VisualElement>("darken");
        exitGameButton = uiDocument.rootVisualElement.Q<Button>("exitGameButton");

        overlayContainer = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainer");
        overlayArray = overlayContainer.Query<VisualElement>("overlay").ToList().ToArray();

        warningBar.style.opacity = 0f;
        pauseMenu.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        escapeKey = new InputSystemHelper(Keyboard.current.escapeKey);
        pauseMenu.style.top = Length.Percent(110);

        exitGameButton.RegisterCallback<ClickEvent>(exitGame);

    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= .3f)
        {
            warningBarFunction();
        }
        if (escapeKey.wasPressedWithCooldown())
        {
            lastEscapePress = Time.unscaledTime;
            escapeClicked = !escapeClicked;
            if (escapeClicked)
            {
                Time.timeScale = 0;
                pauseMenu.style.top = Length.Percent(0);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.7f);
                moveScanLinesCoroutine = StartCoroutine(shiftOverlayRoutine());
            }
            else
            {
                Time.timeScale = 1;
                pauseMenu.style.top = Length.Percent(110);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.0f);
                StopCoroutine(moveScanLinesCoroutine);
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