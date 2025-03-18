using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    //object creation


    //vectors

    //private variables
    private float floatCounter = 360f;

    //public variables
    public bool floatFlag = false;

    // Start is called before the first frame update
    public void calculateItemStart()
    {
        calculateStart();
        //originalPosition = transform.position;
    }

    // Update is called once per frame
    public void calculateItemUpdate()
    {
        calculateUpdate();
        if(floatFlag) floatItem();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(this.gameObject);
    }

    private void floatItem (){
        gravityAffected = true;
        orientToGravity = true;
        /**Vector2 newPosition = new Vector2((transform.position.x + (Mathf.Sin(floatCounter) * .015f) * -gravityDirection.x), (transform.position.y + (Mathf.Sin(floatCounter) * .015f) * -gravityDirection.y));
        rb.MovePosition(newPosition);
        floatCounter -= .05f;
        if (floatCounter <= 0)
            floatCounter = 360;**/
    }

    public void setFloatFlag(bool flag)
    {
        floatFlag = flag;
    }

}
