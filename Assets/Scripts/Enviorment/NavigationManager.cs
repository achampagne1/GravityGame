using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private bool runGenerateNavigationMesh = false;
    [SerializeField] private Vector2 meshCenter = Vector2.zero;
    [SerializeField] private Vector2 navigationMeshSize = new Vector2(50, 50);
    [SerializeField] private float navigationMeshResolution = 1f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject planets;
    [SerializeField] private bool pathDebug = false;
    [SerializeField] private Sprite nodeSprite;
    [SerializeField] private bool drawMesh = false;
    [SerializeField] private int jumpMax = 0;
    private Coroutine navigationMeshGenerationCoroutine;
    private static List<NavigationNode> navigationNodes = new List<NavigationNode>();
    private static List<GravityPoint> gravityPoints = new List<GravityPoint>();
    private static int impassableCost = 10000;
    public static bool meshReady { get; private set; }

    public static NavigationManager instance { get; private set; }

    private void Awake()
    {
        meshReady = false;
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void FixedUpdate()
    {
        if (runGenerateNavigationMesh && navigationMeshGenerationCoroutine == null)
        {
            navigationMeshGenerationCoroutine = StartCoroutine(generateNavigationMesh());
            runGenerateNavigationMesh = false;
            meshReady = true;
        }
    }

    private IEnumerator generateNavigationMesh()
    {
        navigationNodes.Clear();

        int rows = Mathf.Max(1, Mathf.CeilToInt(navigationMeshSize.y * navigationMeshResolution));
        int columns = Mathf.Max(1, Mathf.CeilToInt(navigationMeshSize.x * navigationMeshResolution));

        float startX = -navigationMeshSize.x / 2f;
        float startY = -navigationMeshSize.y / 2f;

        //node creation
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector2 position = meshCenter + new Vector2(
                    startX + (i / navigationMeshResolution),
                    startY + (j / navigationMeshResolution)
                );

                NavigationNode node = new NavigationNode();
                node.position = position;

                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(layerMask);
                filter.useTriggers = true;

                List<Collider2D> results = new List<Collider2D>();

                Physics2D.OverlapPoint(
                    node.position,
                    filter,
                    results
                );

                node.colliders = results.ToArray();

                navigationNodes.Add(node);
            }
        }

        //mesh connection
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                NavigationNode node = navigationNodes[i * rows + j];
                if (j < rows - 1) //right node
                {
                    node.neighbors.Add(navigationNodes[i * rows + (j + 1)]);
                }
                if (i > 0 && j < rows - 1) //top-right diagonal node
                {
                    node.neighbors.Add(navigationNodes[(i - 1) * rows + (j + 1)]);
                }
                if (i < columns - 1) //up node
                {
                    node.neighbors.Add(navigationNodes[(i + 1) * rows + j]);
                }
                if (i > 0 && j > 0) //top-left diagonal node
                {
                    node.neighbors.Add(navigationNodes[(i - 1) * rows + (j - 1)]);
                }
                if (j > 0) //left node
                {
                    node.neighbors.Add(navigationNodes[i * rows + (j - 1)]);
                }
                if (i < columns - 1 && j > 0) //bottom-left diagonal node
                {
                    node.neighbors.Add(navigationNodes[(i + 1) * rows + (j - 1)]);
                }
                if (i > 0) //down node
                {
                    node.neighbors.Add(navigationNodes[(i - 1) * rows + j]);
                }
                if (i < columns - 1 && j < rows - 1) //bottom-right diagonal node
                {
                    node.neighbors.Add(navigationNodes[(i + 1) * rows + (j + 1)]);
                }
            }
        }

        generateGravityPointMap();

        //rule creation
        foreach (NavigationNode node in navigationNodes)
        {
            bool solid = false;
            foreach(Collider2D collider in node.colliders)
            {
                if(collider.gameObject.layer == LayerMask.NameToLayer("gravityField"))
                {
                    node.gravityDirection = collider.transform.rotation * Vector2.down;
                }
                solid = (collider.gameObject.layer == LayerMask.NameToLayer("planet") || collider.gameObject.layer == LayerMask.NameToLayer("platform"));
            }
            if (solid)
            {
                node.movementCost = NAVIGATIONCOSTS.SOLID;
            }
            else
            {
                int depth = neighborIsGround(node);
                if(depth == 0)
                {
                    node.movementCost = NAVIGATIONCOSTS.GROUND;
                }
                else if(depth > 0 && depth <= jumpMax)
                {
                    node.movementCost = NAVIGATIONCOSTS.JUMPABLE;
                }
                else
                {
                    node.movementCost = NAVIGATIONCOSTS.AIR;
                }

            }
        }
        navigationMeshGenerationCoroutine = null;
        yield return null;
    }

    /// <summary>
    /// Checks neighbors of the node and returns true if the neighbor that gravity points to is solid
    /// </summary>
    private static int neighborIsGround(NavigationNode node)
    {
        float minDistance = float.MaxValue;
        Vector2 closestGravityDirection = Vector2.zero;

        if (node.gravityDirection != Vector2.zero) { 
            closestGravityDirection = node.gravityDirection;
        }
        else
        {
            for (int i = 0; i < gravityPoints.Count; i++)
            {
                Vector2 gpPos = new Vector2(gravityPoints[i].x, gravityPoints[i].y);
                float distance = Vector2.Distance(node.position, gpPos) / gravityPoints[i].fieldSize;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestGravityDirection = gpPos - node.position;
                }
            }
        }

        NavigationNode groundNeighbor = null;
        float smallestAngle = float.MaxValue;
        return recursiveNeighborCheck(node, closestGravityDirection, 0);
    }

    private static int recursiveNeighborCheck(NavigationNode homeNode, Vector2 closestGravityDirection,int parentDepth)
    {
        //which neighbor is in the direction of the gravity source
        NavigationNode groundNeighbor = null;
        float smallestAngle = float.MaxValue;
        foreach (NavigationNode neighbor in homeNode.neighbors)
        {
            Vector2 neighborDirection = neighbor.position - homeNode.position;
            float angle = Vector2.Angle(closestGravityDirection, neighborDirection);
            if (angle <= 22.5f && angle < smallestAngle)
            {
                smallestAngle = angle;
                groundNeighbor = neighbor;
            }
        }

        //is the neighbor in the direction a solid
        foreach (Collider2D collider in groundNeighbor.colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("planet") ||
                collider.gameObject.layer == LayerMask.NameToLayer("platform"))
            {
                return parentDepth;
            }
        }

        if (parentDepth >= 5)
        {
            return parentDepth+1;
        }

        return recursiveNeighborCheck(groundNeighbor, closestGravityDirection, parentDepth + 1);
    }

    private static void generateGravityPointMap() {
        gravityPoints.Clear();

        int i = 0;
        foreach (Transform planet in instance.planets.transform)
        {
            GravityPointController controller = planet.GetComponent<GravityPointController>();
            if (controller != null)
            {
                GravityPoint gp = new GravityPoint
                {
                    x = planet.position.x,
                    y = planet.position.y,
                    fieldSize = controller.getFieldSize()
                };

                gravityPoints.Add(gp);
            }
            else
            {
                Debug.LogError($"No GravityPointController attached to {planet.name}");
            }
            i++;
        }
    }

    public static List<Vector2> findPath(Vector2 from,Vector2 to,NavigationProfile navigationProfile)
    {

        // Find the closest nodes to the start and destination.
        NavigationNode startNode = getClosestNode(from);
        NavigationNode targetNode = getClosestNode(to);

        if (startNode == null || targetNode == null)
            return new List<Vector2>();

        // Nodes waiting to be evaluated.
        List<NavigationNode> openSet = new()
    {
        startNode
    };

        // Nodes already evaluated.
        HashSet<NavigationNode> closedSet = new();

        // Tracks the node we came from.
        Dictionary<NavigationNode, NavigationNode> cameFrom = new();

        // Actual cost from start to each node.
        Dictionary<NavigationNode, float> gScore = new()
        {
            [startNode] = 0f
        };

        // Estimated total cost.
        Dictionary<NavigationNode, float> fScore = new()
        {
            [startNode] = getDistance(startNode, targetNode)
        };

        while (openSet.Count > 0)
        {
            // Find the node with the lowest fScore.
            NavigationNode current = openSet[0];

            foreach (NavigationNode node in openSet)
            {
                if (fScore[node] < fScore[current])
                    current = node;
            }

            // Destination reached.
            if (current == targetNode)
            {
                List<Vector2> path = reconstructPath(cameFrom, current, startNode);
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            // Evaluate neighboring nodes.
            foreach (NavigationNode neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor))
                    continue;

                // Treat extremely expensive nodes as impassable.
                if ((int)neighbor.movementCost >= impassableCost)
                    continue;

                // Calculate the cost of reaching this neighbor.
                float tentativeGScore =
                    gScore[current] +
                    Vector2.Distance(current.position, neighbor.position)
                    * (int)neighbor.movementCost;

                // Check whether this is a better route.
                if (gScore.TryGetValue(neighbor, out float previousScore)
                    && tentativeGScore >= previousScore)
                    continue;

                // Record the better route.
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;

                fScore[neighbor] =
                    tentativeGScore + getDistance(neighbor, targetNode);

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
            }
        }

        // No path found.
        return new List<Vector2>();
    }

    private static float getDistance(NavigationNode a,NavigationNode b)
    {
        // Manhattan distance for four-directional movement.
        return Mathf.Abs(a.position.x - b.position.x)
             + Mathf.Abs(a.position.y - b.position.y);
    }

    private static List<Vector2> reconstructPath(Dictionary<NavigationNode, NavigationNode> cameFrom,NavigationNode current,NavigationNode start)
    {
        List<Vector2> path = new();

        while (current != start)
        {
            path.Add(current.position);

            if (!cameFrom.TryGetValue(current, out current))
                return new List<Vector2>();
        }

        path.Reverse();

        return path;
    }

    private static NavigationNode getClosestNode(Vector2 position)
    {
        NavigationNode closest = null;
        float closestDistance = float.MaxValue;

        foreach (NavigationNode node in navigationNodes)
        {
            float distance = Vector2.Distance(position, node.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }

        return closest;
    }

    private void OnDrawGizmos()
    {
        if(instance == null || !instance.drawMesh || !meshReady)
            return;

        foreach(NavigationNode node in navigationNodes)
        {
            if(node.movementCost==NAVIGATIONCOSTS.SOLID)
            {
                Gizmos.color = Color.red;
            }
            else if(node.movementCost == NAVIGATIONCOSTS.GROUND)
            {
                Gizmos.color = Color.green;
            }
            else if(node.movementCost == NAVIGATIONCOSTS.AIR)
            {
                Gizmos.color = Color.blue;
            }
            else if(node.movementCost == NAVIGATIONCOSTS.JUMPABLE)
            {
                Gizmos.color = Color.yellow;
            }
            Gizmos.DrawSphere(node.position, 0.1f);
        }
    }
}
