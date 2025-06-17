using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
public class MainMenuUiHandler : MonoBehaviour
{
    [SerializeField] float speed = .05f;
    [SerializeField] float shiftNum = 4f;
    [SerializeField] float hoverSize = 1.1f;

    private VisualElement overlayContainerEnd;
    private VisualElement[] overlayArrayEnd = new VisualElement[25];
    private VisualElement endScreen;
    private ButtonWrapper tutorial;
    private ButtonWrapper endless;
    private ButtonWrapper options;
    private ButtonWrapper exit;
    private UIDocument uiDocument;
    private AudioSource tabletHumAudioSource;
    private AudioSource buttonClickAudioSource;
    private AudioSource buttonHoverAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        tabletHumAudioSource = GetComponents<AudioSource>()[0];
        buttonClickAudioSource = GetComponents<AudioSource>()[1];
        buttonHoverAudioSource = GetComponents<AudioSource>()[2];
        tutorial = new ButtonWrapper(uiDocument.rootVisualElement.Q<Button>("tutorial"), uiDocument.rootVisualElement.Q<Button>("tutorial2"), () => SceneManager.LoadScene("Tutorial"),buttonClickAudioSource,buttonHoverAudioSource);
        endless = new ButtonWrapper(uiDocument.rootVisualElement.Q<Button>("endless"), uiDocument.rootVisualElement.Q<Button>("endless2"), () => Debug.Log("Endless Clicked"), buttonClickAudioSource,buttonHoverAudioSource);
        options = new ButtonWrapper(uiDocument.rootVisualElement.Q<Button>("options"), uiDocument.rootVisualElement.Q<Button>("options2"), () => Debug.Log("Options Clicked"), buttonClickAudioSource, buttonHoverAudioSource);
        exit = new ButtonWrapper(uiDocument.rootVisualElement.Q<Button>("exit"), uiDocument.rootVisualElement.Q<Button>("exit2"), () => { Debug.Log("Options Clicked"); Application.Quit(); }, buttonClickAudioSource, buttonHoverAudioSource);
        tabletHumAudioSource.Play();
        StartCoroutine(delay());
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    private IEnumerator delay()
    {
        overlayContainerEnd = uiDocument.rootVisualElement.Q<VisualElement>("overlayContainerEnd");
        overlayArrayEnd = overlayContainerEnd.Query<VisualElement>("overlayEnd").ToList().ToArray();
        endScreen = uiDocument.rootVisualElement.Q<VisualElement>("endScreen");

        yield return new WaitForSeconds(.5f);
        StartCoroutine(shiftOverlayRoutine(overlayContainerEnd.resolvedStyle.height, overlayArrayEnd));
    }

    private IEnumerator shiftOverlayRoutine(float parentTop, VisualElement[] overlayArray)
    {
        while (true)
        {
            for (int i = 0; i < 25; i++)
            {
                float topInPixels = overlayArray[i].resolvedStyle.top;
                float topPercent = (topInPixels / parentTop) * 100f;
                float shiftPixels = (shiftNum / 100f) * parentTop;
                if (topPercent > 100)
                    overlayArray[i].style.top = new Length(0, LengthUnit.Pixel);
                else
                    overlayArray[i].style.top = new Length(topInPixels + shiftPixels, LengthUnit.Pixel);
            }
            yield return new WaitForSecondsRealtime(speed);
        }
    }
}
