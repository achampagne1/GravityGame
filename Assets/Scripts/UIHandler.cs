using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public float currentHealth =1f;
    public static UIHandler instance { get; private set; }
    float fadeCounter = 180f;
    VisualElement fullBar;
    VisualElement warningBar;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        fullBar = uiDocument.rootVisualElement.Q<VisualElement>("healthBar");
        warningBar = uiDocument.rootVisualElement.Q<VisualElement>("warningBar");
        warningBar.style.opacity = 0f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentHealth <= .3f)
        {
            warningBarFunction();
        }
    }

    public void setHealthValue(float health)
    {
        currentHealth = health / 10f;
        fullBar.style.width = Length.Percent(currentHealth * 100.0f);
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
