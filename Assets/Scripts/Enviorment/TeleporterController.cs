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
    [SerializeField] float beamDisplayTime = 2f;
    [SerializeField] bool on = false;
    [SerializeField] bool beamOn = false;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite transportBeamSprite;
    [SerializeField] GameObject line;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] AudioClip teleporterClip;
    private GameObject transportBeam;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private List<SpriteRenderer> srLines = new List<SpriteRenderer>();
    private Coroutine moveLinesCoroutine;
    private Coroutine transportBeamCoroutine;
    private bool playerOnPad = false;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        srLines.Add(line.GetComponent<SpriteRenderer>());

        //for creation of teleporter lines
        for (int i = 0; i < lineCount; i++)
        {
            Transform lineHolder = transform.GetChild(0);
            GameObject newLine = Instantiate(line, lineHolder); // Parent first
            newLine.transform.localPosition = line.transform.localPosition + new Vector3(-0.3375332f, i * 0.5f, 0f);
            srLines.Add(newLine.GetComponent<SpriteRenderer>());
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            sr.sprite = onSprite;
            if (moveLinesCoroutine == null)
                moveLinesCoroutine = StartCoroutine(moveLines());
        }
        else
        {
            sr.sprite = offSprite;
            if (moveLinesCoroutine != null)
            {
                StopCoroutine(moveLinesCoroutine);
                moveLinesCoroutine = null;
            }
            foreach (SpriteRenderer line in srLines)
            {
                Color c = line.color;
                c.a = 0f;
                line.color = c;
            }
        }

        if (beamOn)
        {
            if (transportBeamCoroutine == null)
                transportBeamCoroutine = StartCoroutine(teleportBeam());
        }
    }

    private IEnumerator teleportBeam()
    {
        audioSource.clip = teleporterClip;
        audioSource.Play();
        particleSystem.Play();
        createBeam();
        Coroutine shiftBeamCoroutine = StartCoroutine(shiftBeam());
        yield return new WaitForSeconds(beamDisplayTime);
        particleSystem.Stop();
        if (shiftBeamCoroutine != null)
        {
            StopCoroutine(shiftBeamCoroutine);
        }
        shiftBeamCoroutine = null;
        destroyBeam();
        audioSource.Stop();
        beamOn = false;
        transportBeamCoroutine = null;
        yield return null;
    }

    private void createBeam()
    {
        transportBeam = new GameObject("TransportBeam");
        transportBeam.transform.SetParent(transform);
        SpriteRenderer beamSr = transportBeam.AddComponent<SpriteRenderer>();
        beamSr.sprite = transportBeamSprite;
        transportBeam.transform.parent = transform;
        transportBeam.transform.localScale = new Vector3(1f, 1.5f, 1f);
        transportBeam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transportBeam.transform.localPosition = new Vector3(0f, 68.6f, 0f);
    }

    private void destroyBeam()
    {
        if (transportBeam != null)
        {
            Destroy(transportBeam);
            transportBeam = null;
        }
    }

    private IEnumerator shiftBeam()
    {
        while (transportBeam != null)
        {
            Vector3 localScale = transportBeam.transform.localScale;
            transportBeam.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            yield return new WaitForSeconds(.1f);
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

    public void turnOnOff(bool on)
    {
        this.on = on;
    }

    public void triggerBeam()
    {
        beamOn = true;
    }

    public void forceBeamOff()
    {
        beamOn = false;
    }

    public void setPlayerOnPad(bool playerOnPad)
    {
        this.playerOnPad = playerOnPad;
    }

    public bool getPlayerOnPad()
    {
        return playerOnPad;
    }
}
