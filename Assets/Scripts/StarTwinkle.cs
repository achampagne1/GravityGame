using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarTwinkle : MonoBehaviour
{
    [SerializeField] float twinkleScale = 0.0025f; //This sets how big the stars grow to before shrinking back in size
    float twinkleSpeed; //This is how fast the stars "twinkle", it's set in the Start function

    void Start()
    {
        twinkleSpeed = Random.Range(0.025f, 0.1f);
        StartCoroutine(Twinkle());
    }

    IEnumerator Twinkle()
    {
        while(0 == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                transform.localScale += new Vector3(twinkleScale, twinkleScale, 0);
                yield return new WaitForSeconds(twinkleSpeed);
            }

            for (int i = 0; i < 10; i++)
            {
                transform.localScale -= new Vector3(twinkleScale, twinkleScale, 0);
                yield return new WaitForSeconds(twinkleSpeed);
            }

            yield return new WaitForSeconds(Random.Range(1f,3f));
        }
    }
}
