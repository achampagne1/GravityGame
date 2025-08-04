using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class UIHandler : MonoBehaviour
{
    //game variables
    public float movePercent = 0f;
    private float parentTop = 0f;
    private float healthBuffer =0f;
    private bool escapeClicked = false;
    private bool eClicked = false;
    private bool coroutineRunning = false;
    private bool acknowledgeComs = false;
    private bool revealOverride = false;
    private bool up = false;
    private float healthLevel=100f;
    private float shieldLevel=100f;
    [SerializeField] float speed = .05f;
    [SerializeField] float shiftNum = 4f;
    [SerializeField] float screenTextScaler = 100f;
    [SerializeField] float textRevealSpeed = 1f;
    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] Texture2D heartTexture;
    [SerializeField] Texture2D heartBrokenTexture;

    //object creation
    public static UIHandler instance { get; private set; }

    private VisualElement fuelBar;
    private VisualElement shieldBar;
    private VisualElement heartContainer;
    private VisualElement[] hearts = new VisualElement[10];
    private VisualElement warning;
    private VisualElement pauseMenu;
    private VisualElement darken;
    private VisualElement pauseContainer;
    private VisualElement objectiveContainer;
    private VisualElement exitGameButton;
    private VisualElement overlayContainerPause;
    private VisualElement overlayContainerEnd;
    private VisualElement[] overlayArrayPause = new VisualElement[25];
    private VisualElement[] overlayArrayEnd = new VisualElement[25];
    private VisualElement endScreen;
    private VisualElement levelComplete;
    private VisualElement levelFailed;
    private VisualElement comsOverlay;
    private VisualElement textBubble;
    private Label bubbleText;

    private Objective currentObjective;
    private InputSystemHelper escapeKey;
    private InputSystemHelper eKey;
    private InputSystemHelper rKey;

    private AudioSource comsStaticAudioSource;
    private AudioSource comsAcknowledgeAudioSource;
    private AudioSource tabletHumAudioSource;

    private Coroutine moveScanLinesCoroutine = null;
    private Coroutine revealBubbleTextCoroutine = null;
    private Coroutine warningCoroutineVariable = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        fuelBar = uiDocument.rootVisualElement.Q<VisualElement>("fuel");
        shieldBar = uiDocument.rootVisualElement.Q<VisualElement>("shield");
        heartContainer = uiDocument.rootVisualElement.Q<VisualElement>("heartContainer");
        hearts = heartContainer.Query<VisualElement>("heart").ToList().ToArray();
        warning = uiDocument.rootVisualElement.Q<VisualElement>("warning");
        pauseMenu = uiDocument.rootVisualElement.Q<VisualElement>("pause");
        pauseContainer = uiDocument.rootVisualElement.Q<VisualElement>("pauseContainer");
        darken = uiDocument.rootVisualElement.Q<VisualElement>("darken");
        exitGameButton = uiDocument.rootVisualElement.Q<Button>("exitGameButton");
        objectiveContainer = uiDocument.rootVisualElement.Q<VisualElement>("text");
        overlayContainerPause = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainer");
        overlayArrayPause = overlayContainerPause.Query<VisualElement>("overlay").ToList().ToArray();
        overlayContainerEnd = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainerEnd");
        overlayArrayEnd = overlayContainerEnd.Query<VisualElement>("overlayEnd").ToList().ToArray();
        endScreen = uiDocument.rootVisualElement.Q<VisualElement>("endScreen");
        levelComplete = uiDocument.rootVisualElement.Q<VisualElement>("levelComplete");
        levelFailed = uiDocument.rootVisualElement.Q<VisualElement>("levelFailed");
        comsOverlay = uiDocument.rootVisualElement.Q<VisualElement>("coms");
        textBubble = uiDocument.rootVisualElement.Q<VisualElement>("textBubble");
        bubbleText = uiDocument.rootVisualElement.Q<Label>("bubbleText");
        pauseContainer.style.opacity = 0f;
        objectiveContainer.style.opacity = 0f;
        pauseMenu.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        comsOverlay.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        escapeKey = new InputSystemHelper(Keyboard.current.escapeKey);
        eKey = new InputSystemHelper(Keyboard.current.eKey);
        rKey = new InputSystemHelper(Keyboard.current.rKey);
        pauseMenu.style.top = Length.Percent(110);
        comsStaticAudioSource = (GetComponents<AudioSource>())[0];
        comsAcknowledgeAudioSource = (GetComponents<AudioSource>())[1];
        tabletHumAudioSource = (GetComponents<AudioSource>())[2];

        for(int i = 0; i < 10; i++)
        {
            hearts[i].style.left = Length.Percent(i*10.1f); //10.1 is the amount each heart is shifted by
        }

        warning.style.opacity = 0f;

        exitGameButton.RegisterCallback<ClickEvent>(exitGame); //gotta figure this out
    }

    // Update is called once per frame
    void Update()
    {
        bubbleText.style.fontSize = Screen.width / screenTextScaler;
        if (shieldLevel == 0 && warningCoroutineVariable == null)
            warningCoroutineVariable = StartCoroutine(warningCoroutine());
        else if(shieldLevel!=0&& warningCoroutineVariable != null)
        {
            warning.style.opacity = 0f;
            StopCoroutine(warningCoroutineVariable);
            warningCoroutineVariable = null;
        }

        if (escapeKey.wasPressedWithCooldown()&&!eClicked)
        {
            escapeClicked = !escapeClicked;
            if (escapeClicked)
            {
                //Time.timeScale = 0; //this pauses the scan lines too for some reason
                StartCoroutine(fadeInHum());
                pauseMenu.style.top = Length.Percent(0);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.7f);
                moveScanLinesCoroutine = StartCoroutine(shiftOverlayRoutine(overlayContainerPause.resolvedStyle.height,overlayArrayPause));
                pauseContainer.style.opacity = 1;
            }
            else
            {
                Time.timeScale = 1;
                StartCoroutine(fadeOutHum());
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
                StartCoroutine(fadeInHum());
                pauseMenu.style.top = Length.Percent(movePercent);
                moveScanLinesCoroutine = StartCoroutine(shiftOverlayRoutine(overlayContainerPause.resolvedStyle.height, overlayArrayPause));
                objectiveContainer.style.opacity = 1;
            }
            else
            {
                StartCoroutine(fadeOutHum());
                pauseMenu.style.top = Length.Percent(110);
                StopCoroutine(moveScanLinesCoroutine);
                objectiveContainer.style.opacity = 0;
            }
        }
        //coms lokcout is needed so the player cant retract coms when new text is supposed to be displayed
        //it automatically "pulls it back down" however, if there is more text inconversation 
        if (rKey.wasPressedWithCooldown() && up)
        {
            comsAcknowledgeAudioSource.PlayOneShot(comsAcknowledgeAudioSource.clip);
            if (revealBubbleTextCoroutine == null)
            {
                acknowledgeComs = true;
                coms(false);
            }
            else //meaning if reveal text is still running
            {
                revealOverride = true;
            }
        }
        else
            acknowledgeComs = false;
    }

    private IEnumerator shiftOverlayRoutine(float parentTop,VisualElement[] overlayArray)
    {
        while (true)
        {
            for (int i = 0; i < 25; i++)
            {
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
        Application.Quit();
    }

    //if complete is true then level complete show. if not then level failed
    public void showLevelEnd(bool complete) //maybe chnge this to a bool, however, level end should only be displayed once
    {
        if (complete)
        {
            levelComplete.style.opacity = 1f;
            levelFailed.style.opacity = 0f;
        }
        else
        {
            levelComplete.style.opacity = 0f;
            levelFailed.style.opacity = 1f;
        }
        endScreen.style.top = 0f;
        StartCoroutine(shiftOverlayRoutine(overlayContainerEnd.resolvedStyle.height, overlayArrayEnd));
    }

    public void coms(bool up)
    {
        this.up = up;
        if (!up)
        {
            comsOverlay.style.top = Length.Percent(100f);
        }
        else
            comsOverlay.style.top = 0f;
    }

    private IEnumerator fadeInHum()
    {
        tabletHumAudioSource.Play();
        tabletHumAudioSource.volume = 0f;
        while (tabletHumAudioSource.volume < .1f)
        {
            tabletHumAudioSource.volume += .02f;
            yield return new WaitForSeconds(.00075f);
        }
    }

    private IEnumerator fadeOutHum()
    {
        while (tabletHumAudioSource.volume > 0f)
        {
            tabletHumAudioSource.volume -= .02f;
            yield return new WaitForSeconds(.00075f);
        }
        tabletHumAudioSource.Stop();
    }

    private IEnumerator warningCoroutine()
    {
        while (true)
        {
            float opacity = (Mathf.Sin(Time.time*fadeSpeed) + 1f) / 2f;
            warning.style.opacity = opacity;
            yield return new WaitForSeconds(.05f);
        }
    }

    public void setHealthValue(float health)
    {
        if (healthBuffer == health)
            return;

        healthBuffer = health;
        health -= 1;
        for(int i = 9; i > health; i--)
        {
            hearts[i].style.backgroundImage = new StyleBackground(heartBrokenTexture);
        }
    }

    public void setFuelValue(float fuelLevel)
    {
        float initialOffset = 42f; //I dont know why its this number
        float finalOffset = 54f;
        float fuelScaler = .6f;
        fuelLevel *= fuelScaler;
        fuelBar.style.width = Length.Percent(fuelLevel);
        fuelBar.style.left = Length.Percent(finalOffset - (fuelLevel / 100f) * (finalOffset - initialOffset));
    }
    public void setShieldValue(float shieldLevel)
    {
        float initialOffset = -41f; //I dont know why its this number
        float finalOffset = 46f;
        float shieldScaler = .6f;
        shieldLevel *= shieldScaler;
        shieldBar.style.width = Length.Percent(shieldLevel);
        shieldBar.style.left = Length.Percent(finalOffset - (shieldLevel / 100f) * (finalOffset - initialOffset));
        this.shieldLevel= shieldLevel;
    }

    public void setBubbleText(string text) //height for 1 line is 8 with top at 83, 2 lines is 14 with top at 79, 3 lines is 20 with top at 73
    {
        revealBubbleTextCoroutine = StartCoroutine(bubbleTextReveal(text));
    }

    private IEnumerator bubbleTextReveal(string text)
    {
        if (revealBubbleTextCoroutine != null)
            StopCoroutine(revealBubbleTextCoroutine);

        coms(true); //pull up coms
        bubbleText.text = ""; //resets the text bubble

        comsStaticAudioSource.Play();
        foreach (char character in text)
        {
            if (!revealOverride)
            {
                bubbleText.text += character;
                yield return new WaitForSecondsRealtime(textRevealSpeed);
            }
            else
            {
                bubbleText.text = text; //reveals all text
                revealOverride = false;
                break;
            }
        }
        comsStaticAudioSource.Stop();
        revealBubbleTextCoroutine = null;
    }

    public void setCurrentObjective(Objective currentObjective)
    {
        this.currentObjective = currentObjective;
        objectiveContainer.style.backgroundImage = currentObjective.visualElement.resolvedStyle.backgroundImage;
    }

    public bool getAcknowledgeComs()
    {
        bool temp = acknowledgeComs;
        acknowledgeComs = false;
        return temp;
    }
}