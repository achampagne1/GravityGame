using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

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
    private bool acknowledgeComs = false;
    private bool revealOverride = false;
    [SerializeField] float speed = .05f;
    [SerializeField] float shiftNum = 4f;
    [SerializeField] float screenTextScaler = 100f;
    [SerializeField] float textRevealSpeed = 1f;
    [SerializeField] AudioClip radioStaticClip;
    [SerializeField] AudioClip tabletHum;
    [SerializeField] AudioClip comsAcknowledge;

    //object creation
    public static UIHandler instance { get; private set; }

    private VisualElement fullBar;
    private VisualElement fullFuelBar;
    private VisualElement warningBar;
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
    private VisualElement comsOverlay;
    private VisualElement textBubble;
    private Label bubbleText;

    private ObjectiveWrapper objectiveWrapper;
    private Objective currentObjective;
    private InputSystemHelper escapeKey;
    private InputSystemHelper eKey;
    private InputSystemHelper rKey;
    private AudioSource audioSource;

    private Coroutine moveScanLinesCoroutine;
    private Coroutine revealBubbleTextCoroutine;

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
        overlayContainerPause = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainer");
        overlayArrayPause = overlayContainerPause.Query<VisualElement>("overlay").ToList().ToArray();
        overlayContainerEnd = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainerEnd");
        overlayArrayEnd = overlayContainerEnd.Query<VisualElement>("overlayEnd").ToList().ToArray();
        endScreen = uiDocument.rootVisualElement.Q<VisualElement>("endScreen");
        comsOverlay = uiDocument.rootVisualElement.Q<VisualElement>("coms");
        textBubble = uiDocument.rootVisualElement.Q<VisualElement>("textBubble");
        bubbleText = uiDocument.rootVisualElement.Q<Label>("bubbleText");
        warningBar.style.opacity = 0f;
        pauseContainer.style.opacity = 0f;
        objectiveContainer.style.opacity = 0f;
        pauseMenu.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        comsOverlay.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        escapeKey = new InputSystemHelper(Keyboard.current.escapeKey);
        eKey = new InputSystemHelper(Keyboard.current.eKey);
        rKey = new InputSystemHelper(Keyboard.current.rKey);
        pauseMenu.style.top = Length.Percent(110);
        audioSource = GetComponent<AudioSource>();

        exitGameButton.RegisterCallback<ClickEvent>(exitGame); //gotta figure this out
    }

    // Update is called once per frame
    void Update()
    {
        bubbleText.style.fontSize = Screen.width / screenTextScaler;

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
        if (rKey.wasPressedWithCooldown())
        {
            audioSource.PlayOneShot(comsAcknowledge);
            if(revealBubbleTextCoroutine == null)
            {
                acknowledgeComs = true;
                coms(false);
            }
            else //meaning if reveal text is still running
            {
                revealOverride = true;
            }
        }
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
        Debug.Log("here");
        Application.Quit();
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

    public void showLevelEnd() //maybe chnge this to a bool, however, level end should only be displayed once
    {
        endScreen.style.top = 0f;
        StartCoroutine(shiftOverlayRoutine(overlayContainerEnd.resolvedStyle.height, overlayArrayEnd));
    }

    public void coms(bool up)
    {
        if (!up)
        {
            comsOverlay.style.top = Length.Percent(100f);
        }
        else
            comsOverlay.style.top = 0f;
    }

    private IEnumerator fadeInHum()
    {
        audioSource.clip = tabletHum;
        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < .5f)
        {
            audioSource.volume += .1f;
            yield return new WaitForSeconds(.00075f);
        }
    }

    private IEnumerator fadeOutHum()
    {
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= .1f;
            yield return new WaitForSeconds(.00075f);
        }
        audioSource.Stop();
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

    public void setBubbleText(string text,float width, float height) //height for 1 line is 8 with top at 83, 2 lines is 14 with top at 79, 3 lines is 20 with top at 73
    {
        revealBubbleTextCoroutine = StartCoroutine(bubbleTextReveal(text, width, height));
    }

    private IEnumerator bubbleTextReveal(string text, float width, float height)
    {
        if (revealBubbleTextCoroutine != null)
            StopCoroutine(revealBubbleTextCoroutine);

        coms(true); //pull up coms
        textBubble.style.width = Length.Percent(width);
        textBubble.style.left = Length.Percent(90 - width);//this is to adjust for the end of the bubble
        textBubble.style.height = Length.Percent(height);
        textBubble.style.top = Length.Percent(88.336f - .667f * height);//this is to adjust the height of the bubble
        bubbleText.text = ""; //resets the text bubble

        audioSource.clip = radioStaticClip;
        audioSource.Play();
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
        audioSource.Stop();
        revealBubbleTextCoroutine = null;
    }

    public Objective getCurrentObjective()
    {
        //NOTE: should objectives be soley handled by the level manager and the ui is only responsible for setting the text?
        return currentObjective;
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