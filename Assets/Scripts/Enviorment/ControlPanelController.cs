using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelController : MonoBehaviour
{
    [SerializeField] float flashRate = 1.0f;
    [SerializeField] int minOn = 3;
    [SerializeField] List<Material> colors;
    [SerializeField] Material off;
    [SerializeField] GameObject normalButton;
    [SerializeField] GameObject depressedButton;
    [SerializeField] bool press = false;
    private bool pressLatch = false;
    private GameObject depressedButtonInstance;
    private List<SpriteRenderer> squares = new List<SpriteRenderer>();
    private Coroutine flashingCoroutine;
    void Start()
    {
        normalButton.transform.localPosition = new Vector2(2.35f, 2f);
        depressedButtonInstance = Instantiate(depressedButton, transform);
        depressedButtonInstance.transform.localPosition = new Vector2(2.45f, 2f);
        normalButton.SetActive(!press);
        depressedButtonInstance.SetActive(press);
        pressLatch = press;

        foreach(Transform child in transform)
        {
            if(child.name == "Squares")
            {
                foreach(Transform square in child)
                {
                    squares.Add(square.GetComponent<SpriteRenderer>());
                }
            }
        }

        flashingCoroutine = StartCoroutine(flashing());
    }

    void Update()
    {
        if (press != pressLatch)
        {
            normalButton.SetActive(!press);
            depressedButtonInstance.SetActive(press);
            pressLatch = press;
        }
    }

    private IEnumerator flashing()
    {
        while (true)
        {
            foreach (SpriteRenderer square in squares)
            {
                square.material = off;
            }

            int amount = UnityEngine.Random.Range(minOn, squares.Count);
            for (int i = 0; i < amount; i++)
            {
                int index = UnityEngine.Random.Range(0, squares.Count);
                int colorIndex = UnityEngine.Random.Range(0, colors.Count);
                squares[i].material = colors[colorIndex];
            }
            yield return new WaitForSeconds(flashRate);
        }
    }
}
