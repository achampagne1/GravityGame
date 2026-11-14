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
    [SerializeField] bool pathDebug = false;
    [SerializeField] float nodeArrivalDistance = 0.1f;
    [SerializeField] float jumpThreshold = .1f;

    //game variables
    private int moveInput = 0;
    private bool pause = false;
    private bool following = false;
    private bool attackLatch = false;
    private Vector3 playerDirection = new Vector3(0f, 0f, 0f);
    private Coroutine navigationCoroutine = null;
    private Coroutine generatePathCoroutine = null;
    private List<Vector2> path = null;


    public override void Start()
    {
        base.Start();
    }

    public void FixedUpdate()
    {
        if (NavigationManager.meshReady && generatePathCoroutine == null)
        {
            generatePathCoroutine = StartCoroutine(generatePath());
        }

        if (findPath && navigationCoroutine == null && path != null)
        {
            findPath = false;
            navigationCoroutine = StartCoroutine(navigation(path));

        }
        base.FixedUpdate();
    }

    private IEnumerator generatePath()
    {
        path = NavigationManager.findPath(transform.position, end, null);
        Debug.Log("Path generated with " + path.Count + " nodes.");
        yield return null;
    }

    private IEnumerator navigation(List<Vector2> nodes)
    {
        int iterator = 1;
        while (iterator < nodes.Count)
        {
            space = false;
            Vector2 currentNode = nodes[iterator];
            Vector2 nodeOffset = Quaternion.Inverse(transform.rotation) * (currentNode - (Vector2)transform.position);

            if (Mathf.Abs(nodeOffset.x) <= nodeArrivalDistance || Mathf.Abs(nodeOffset.y) <= nodeArrivalDistance)
            {
                iterator++;
            }
            else
            {
                int direction = nodeOffset.x > 0f ? 1 : nodeOffset.x < 0f ? -1 : 0;
                setMovement(direction);
                setOrientation(direction);

                if (nodeOffset.y > jumpThreshold)
                {
                    space = true;
                }
            }
            yield return null;
        }

        setMovement(0);
        navigationCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (path == null || !pathDebug)
            return; 

        foreach (Vector2 nodePosition in path)
        {
            Gizmos.DrawSphere(nodePosition, 0.1f);
        }
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