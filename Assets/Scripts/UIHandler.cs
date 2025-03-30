using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    public float currentHealth =1f;
    private StyleLength startTop;
    private float shift = 0;
    public float totalShift = 5f;
    public static UIHandler instance { get; private set; }
    private float fadeCounter = 180f;
    private bool escapeClicked = false;
    private float lastEscapePress = 0f;
    VisualElement fullBar;
    VisualElement fullFuelBar;
    VisualElement warningBar;
    VisualElement pauseMenu;
    VisualElement darken;
    VisualElement overlay;
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
        overlay = uiDocument.rootVisualElement.Q<VisualElement>("overlay");
        warningBar.style.opacity = 0f;
        pauseMenu.style.transitionDuration = new List<TimeValue> { new TimeValue(0.25f, TimeUnit.Second) };
        pauseMenu.style.top = Length.Percent(110);
        startTop = overlay.resolvedStyle.top;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= .3f)
        {
            warningBarFunction();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.escapeKey.isPressed && Time.unscaledTime - lastEscapePress > 0.15f)
        {
            escapeClicked = !escapeClicked;
            if (escapeClicked)
            {
                //Time.timeScale = 0;
                pauseMenu.style.top = Length.Percent(0);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.7f);
                moveScanLinesCoroutine = StartCoroutine(ShiftOverlayRoutine());
            }
            else
            {
                //Time.timeScale = 1;
                pauseMenu.style.top = Length.Percent(110);
                darken.style.backgroundColor = new Color(0, 0, 0, 0.0f);
                StopCoroutine(moveScanLinesCoroutine);
            }
            lastEscapePress = Time.unscaledTime;
        }
    }

    private IEnumerator ShiftOverlayRoutine()
    {
        while (true)
        {
            // Extract current top position
            float currentTop = overlay.resolvedStyle.top;  // This is read-only, so we get it to calculate
            float newTop = currentTop + 1f; // Move down by 1 pixel

            // Apply the new position using a StyleLength object
            overlay.style.top = new Length(newTop, LengthUnit.Pixel);
            shift++;

            if(shift == totalShift)
            {
                shift = 0;
                overlay.style.top = startTop;
            }
            yield return new WaitForSeconds(.05f);// Adjust delay for smoother shifting
        }
    }


    /**private IEnumerator shiftOverlay(VisualElement overlay, float shiftAmount, float duration)
    {
        float elapsedTime = 0f;
        float startTop = overlay.resolvedStyle.top.value; // Get the initial position
        float targetTop = startTop + shiftAmount;

        while (elapsedTime < duration)
        {
            float newTop = Mathf.Lerp(startTop, targetTop, elapsedTime / duration);
            overlay.style.top = new Length(newTop, LengthUnit.Percent); // Corrected
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time in case the game is paused
            yield return null;
        }

        // Snap back to original position
        overlay.style.top = new Length(startTop, LengthUnit.Percent);
    }**/


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
        if(currentHealth == .3f)
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
