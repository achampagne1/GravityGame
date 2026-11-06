using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : SpacePersonController
{

    //public variables
    [SerializeField] int normalState = 0;
    //[SerializeField] Vector2 start = new Vector2();
    [SerializeField] Vector2 end = new Vector2();
    [SerializeField] bool findPath = false;

    //game variables
    private int moveInput = 0;
    private bool pause = false;
    private bool following = false;
    private bool attackLatch = false;
    private Vector3 playerDirection = new Vector3(0f, 0f, 0f);
    private Coroutine navigationCoroutine = null;


    public override void Start()
    {
        base.Start();
    }

    public void FixedUpdate()
    {
        if (findPath && navigationCoroutine == null)
        {
            findPath = false;
            navigationCoroutine = StartCoroutine(navigation(NavigationManager.findPath(transform.position, end, null)));

        }
        //base.FixedUpdate();
    }

    private IEnumerator navigation(List<Vector2> nodes)
    {
        Vector2 currentNode = nodes[0];
        int iterator = 1;
        while(currentNode != nodes[nodes.Count - 1])
        {
            currentNode = nodes[iterator++];
            transform.position = (Vector3)currentNode;
            yield return new WaitForSeconds(.1f);
        }
        navigationCoroutine = null;
        yield return null;
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