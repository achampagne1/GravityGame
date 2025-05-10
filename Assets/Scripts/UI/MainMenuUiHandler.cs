using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainMenuUiHandler : MonoBehaviour
{
    [SerializeField] float speed = .05f;
    [SerializeField] float shiftNum = 4f;

    private VisualElement overlayContainerEnd;
    private VisualElement[] overlayArrayEnd = new VisualElement[25];
    private VisualElement endScreen;
    private Button tutorial;
    private UIDocument uiDocument;

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        tutorial = uiDocument.rootVisualElement.Q<Button>("tutorial");
        tutorial.clicked += () =>
        {
            Debug.Log("here");
            SceneManager.LoadScene("Tutorial");
        };
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
        //tabletHumAudioSource = (GetComponents<AudioSource>())[2];

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
            yield return new WaitForSecondsRealtime(speed);// Adjust delay for smoother shifting
        }
    }
}
