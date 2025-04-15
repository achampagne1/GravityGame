using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    [SerializeField] float lineMoveAmount = .1f;
    [SerializeField] float lineHeight = 1f;
    [SerializeField] float loopTime = .1f;
    [SerializeField] float startPoint = 2.5f;
    [SerializeField] bool toggleState = false;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    private SpriteRenderer sr;
    private List<SpriteRenderer> srLines = new List<SpriteRenderer>();
    private Coroutine moveLinesCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = offSprite;
        foreach (Transform line in transform)
        {
            srLines.Add(line.gameObject.GetComponent<SpriteRenderer>());
        }
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
            toggleState = false;
        }
    }

    private IEnumerator moveLines()
    {
        while (true)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform line = transform.GetChild(i);
                SpriteRenderer srLine = srLines[i];
                line.localPosition = line.localPosition + new Vector3(0f, lineMoveAmount, 0f);
                Color c = srLine.color;
                float distance = Mathf.Abs(line.localPosition.y - startPoint);

                float falloff = Mathf.Log10(lineHeight + 1) - Mathf.Log10(distance + 1);
                float maxFalloff = Mathf.Log10(lineHeight + 1); // Max possible falloff
                c.a = Mathf.Clamp01(falloff / maxFalloff);
                if (line.localPosition.y >= lineHeight)
                {
                    line.localPosition = new Vector3(0f, startPoint, 0f);
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
}
