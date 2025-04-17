using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    [SerializeField] float lineMoveAmount = .1f;
    [SerializeField] float lineHeight = 1f;
    [SerializeField] float loopTime = .1f;
    [SerializeField] float startPoint = 2.5f;
    [SerializeField] float lineCount = 5;
    [SerializeField] bool toggleState = false;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    [SerializeField] GameObject line;
    [SerializeField] GameObject transportBeam;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float beamDisplayTime = 2f;
    private SpriteRenderer sr;
    private List<SpriteRenderer> srLines = new List<SpriteRenderer>();
    private SpriteRenderer transportBeamRenderer;
    private Transform transportBreamTransform;
    private Coroutine moveLinesCoroutine;
    private bool trigger = false;
    private bool on = false;
    private bool teleportLatch = true;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        srLines.Add(line.GetComponent<SpriteRenderer>());
        for (int i = 0; i < lineCount; i++)
        {
            Transform lineHolder = transform.GetChild(0);
            GameObject newLine = Instantiate(line, lineHolder); // Parent first
            newLine.transform.localPosition = line.transform.localPosition + new Vector3(-0.3375332f, i * 0.5f, 0f);
            srLines.Add(newLine.GetComponent<SpriteRenderer>());
        }

        transportBeamRenderer = transportBeam.GetComponent<SpriteRenderer>();
        transportBreamTransform = transportBeam.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleState)
        {
            if (sr.sprite == onSprite)
            {
                sr.sprite = offSprite;
                StopCoroutine(moveLinesCoroutine);
                foreach (SpriteRenderer line in srLines)
                {
                    Color c = line.color;
                    c.a = 0f;
                    line.color = c;
                }
            }
            else
            {
                sr.sprite = onSprite;
                moveLinesCoroutine = StartCoroutine(moveLines());
            }
            on = !on; //reverses the state of the teleporter on or off
            toggleState = false;
        }

        if (trigger && on && teleportLatch) //redo this get the "getcomponent" out
        {
            StartCoroutine(teleportBeam());
            teleportLatch = false;
        }
    }

    private IEnumerator teleportBeam()
    {
        Color c = transportBeamRenderer.color;
        c.a = 1f;
        particleSystem.Play();
        Coroutine shiftBeamCoroutine = StartCoroutine(shiftBeam());
        transportBeamRenderer.color = c;
        yield return new WaitForSeconds(beamDisplayTime);
        c.a = 0f;
        transportBeamRenderer.color = c;
        particleSystem.Stop();
        StopCoroutine(shiftBeamCoroutine);
        teleportLatch = true;
    }

    private IEnumerator shiftBeam()
    {
        while (true)
        {
            transportBreamTransform.localScale = new Vector3(-transportBreamTransform.localScale.x, transportBreamTransform.localScale.y, 0);
            yield return new WaitForSeconds(.075f);
        }
    }

    private IEnumerator moveLines()
    {
        while (true)
        {
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                Transform line = transform.GetChild(0).GetChild(i);
                SpriteRenderer srLine = srLines[i];
                line.localPosition = line.localPosition + new Vector3(0f, lineMoveAmount, 0f);
                Color c = srLine.color;
                float distance = Mathf.Abs(line.localPosition.y - startPoint);

                float falloff = Mathf.Log10(lineHeight + 1+2) - Mathf.Log10(distance + 1);
                float maxFalloff = Mathf.Log10(lineHeight + 1); // Max possible falloff
                c.a = Mathf.Clamp01(falloff / maxFalloff);
                if (line.localPosition.y >= lineHeight)
                {
                    line.localPosition = new Vector3(-0.3375332f, startPoint, 0f);
                    c.a = 1f;
                }
                srLine.color = c;
            }
            yield return new WaitForSeconds(loopTime);
        }
    }

    public void toggleStateFunc()
    {
        toggleState = true;
    }

    public void setTransportTrigger(bool trigger)
    {
        this.trigger = trigger;
    }
}
