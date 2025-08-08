using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : SpacePersonController
{

    //public variables
    [SerializeField] int normalState = 0;

    //game variables
    private int moveInput = 0;
    private bool pause = false;
    private bool following = false;
    private bool attackLatch = false;
    private Vector3 playerDirection = new Vector3(0f, 0f, 0f);

    void Start()
    {
        calculateSpacePersonStart();
    }

    public void FixedUpdate()
    {
        calculateSpacePersonUpdate();
    }

    private int lookLeftOrRight()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter;

        if (direction.x < 0)
            return -1;
        else
            return 1;
    }

    protected override IEnumerator die()
    {
        yield return base.die();
        /*moveInput = 0;
        setMovement(moveInput);
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        Destroy(collider);
        yield return new WaitForSeconds(persistanceAfterDeath);
        Destroy(gameObject);*/
    }
}   