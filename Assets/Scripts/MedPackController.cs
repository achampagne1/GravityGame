using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedPackController : ObjectController
{
    //object creation


    //vectors
    //Vector2 originalPosition = new Vector2(0, 0);

    float floatCounter = 360f;


    // Start is called before the first frame update
    void Start()
    {
        calculateStart();
        //originalPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateUpdate();
        Vector2 newPosition = new Vector2((transform.position.x + (Mathf.Sin(floatCounter) * .015f)*-gravityDirection.x), (transform.position.y+ (Mathf.Sin(floatCounter) * .015f) * -gravityDirection.y));
        rb.MovePosition(newPosition);
        floatCounter -=.05f;
        if (floatCounter <= 0)
            floatCounter = 360;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);

    }
}
