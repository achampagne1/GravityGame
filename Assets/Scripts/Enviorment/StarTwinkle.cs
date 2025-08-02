using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarTwinkle : MonoBehaviour
{
    [SerializeField] float twinkleScale = 0.0025f; //This sets how big the stars grow to before shrinking back in size
    [SerializeField] Transform followPoint;
    private Vector3 lastPlayerPosition;
    private float twinkleSpeed; //This is how fast the stars "twinkle", it's set in the Start function
    private float scale = 1f;

    void Start()
    {
        twinkleSpeed = Random.Range(0.025f, 0.1f);
        followPoint = MainCameraChecker.mainCameraLocation;
        lastPlayerPosition = followPoint.position;
        StartCoroutine(Twinkle());
    }

    void Update()
    {
        followPoint = MainCameraChecker.mainCameraLocation;
        if (followPoint != null)
        {
            Vector3 delta = followPoint.position - lastPlayerPosition;
            transform.position += delta * scale;
            lastPlayerPosition = followPoint.position;
        }
    }

    IEnumerator Twinkle()
    {
        if (gameObject.tag == "twinkle")
        {
            while (0 == 0)
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

                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
        }
    }

    public void setFollowPoint(Transform followPoint)
    {
        this.followPoint=followPoint;
    }

    public void setScale(float scale)
    {
        this.scale = scale;
    }
}
