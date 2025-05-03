using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    //object creation


    //vectors
    private Vector3 originalPosition = Vector3.zero;

    //private variables
    private float floatCounter = 360f;
    private Coroutine floatItemCoroutine;

    //public variables
    public bool floatFlag = false;
    [SerializeField] float magnitudeOfFloat = .25f;
    [SerializeField] float flaotSpeed = 100f;

    // Start is called before the first frame update
    public void calculateItemStart()
    {
        calculateStart();
        originalPosition = transform.position;
    }

    // Update is called once per frame
    public void calculateItemUpdate()
    {
        calculateUpdate();
        if(floatFlag && floatItemCoroutine==null)
            floatItemCoroutine = StartCoroutine(floatItem());
        else if(!floatFlag && floatItemCoroutine != null)
        {
            StopCoroutine(floatItemCoroutine);
            floatItemCoroutine = null;
        }
    }

    private IEnumerator floatItem()
    {
        orientToGravity = true;
        originalPosition = transform.position;
        while (true)
        {
            floatCounter -= flaotSpeed * Time.deltaTime;

            Vector2 newPosition = new Vector2(
                originalPosition.x + Mathf.Sin(floatCounter) * magnitudeOfFloat * -gravityDirection.x,
                originalPosition.y + Mathf.Sin(floatCounter) * magnitudeOfFloat * -gravityDirection.y
            );

            rb.MovePosition(newPosition);

            if (floatCounter <= 0)
            {
                floatCounter = 360f;
                transform.position = originalPosition;
            }

            yield return null; 
        }
    }

    public void setFloatFlag(bool flag)
    {
        floatFlag = flag;
    }

}
