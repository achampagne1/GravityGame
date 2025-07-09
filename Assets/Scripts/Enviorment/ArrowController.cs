using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;
    [SerializeField] private Sprite glow;
    [SerializeField] private bool onState = false;
    [SerializeField] private bool glowState = false;
    [SerializeField] private float fadeSpeed = 1f;
    private Coroutine fadeRoutine;
    private float fadeStepSize = .1f;
    private float fadeSize = 0;
    private float startValue = 0.0f;

    private SpriteRenderer sr;
    private SpriteRenderer glowSr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        glowSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!onState)
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
                fadeRoutine = null;
            }
            sr.sprite = off; //changes the main sprite to off
            Color glowColor = glowSr.color; //changes alpha channle for glow
            glowColor.a = 0;
            glowSr.color = glowColor;
        }
        else
        {
            if (!glowState)
            {
                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                    fadeRoutine = null;
                }
                sr.sprite = on; //changes main sprite to on
                Color glowColor = glowSr.color; //changes alpha channle for glow
                glowColor.a = 0;
                glowSr.color = glowColor;
            }
            else
            {
                sr.sprite = off; //changes main sprite to off
                if (fadeRoutine == null)
                {
                    Debug.Log("here");
                    fadeRoutine = StartCoroutine(glowFunc());
                }
            }
        }
    }

    private IEnumerator glowFunc()
    {
        fadeSize = startValue; 

        while (true)
        {
            Color glowColor = glowSr.color;
            glowColor.a = 0.5f * (Mathf.Cos(fadeSize) + 1f);
            glowSr.color = glowColor;

            fadeSize += fadeStepSize;
            if (fadeSize > 2f * Mathf.PI)
                fadeSize = 0;

            yield return new WaitForSeconds(fadeSpeed);
        }
    }

    public void setState(bool state)
    {
        onState = state;
        glowState = state;//changes it form on to glowing
    }

    public void setStartValue(float offset)
    {
        startValue = offset % (2f * Mathf.PI);
    }

    public void setFadeSpeed(float fadeSpeed)
    {
        this.fadeSpeed = fadeSpeed;
    }
}
