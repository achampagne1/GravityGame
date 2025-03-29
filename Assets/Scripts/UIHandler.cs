using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    public float currentHealth =1f;
    public static UIHandler instance { get; private set; }
    private float fadeCounter = 180f;
    private bool escapeClicked = false;
    private float lastEscapePress = 0f;
    VisualElement fullBar;
    VisualElement fullFuelBar;
    VisualElement warningBar;
    VisualElement pauseMenu;

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
        warningBar.style.opacity = 0f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentHealth <= .3f)
        {
            warningBarFunction();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.escapeKey.isPressed && Time.unscaledTime - lastEscapePress > 0.15f)
        {
            lastEscapePress = Time.unscaledTime;
            escapeClicked = !escapeClicked;
            pauseMenu.style.top = escapeClicked ? Length.Percent(-10) : Length.Percent(110);
        }
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

    void warningBarFunction()
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
