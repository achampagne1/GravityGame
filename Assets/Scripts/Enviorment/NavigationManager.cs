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
    [SerializeField] private bool pathDebug;
    [SerializeField] private Sprite nodeSprite;
    private Coroutine navigationMeshGenerationCoroutine;
    private static List<NavigationNode> navigationNodes = new List<NavigationNode>();
    private static List<GravityPoint> gravityPoints = new List<GravityPoint>();
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

                node.colliders = Physics2D.OverlapPointAll(
                    node.position,
                    layerMask
                );

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
                    node.neighbors.Add((new Vector2(337.5f,22.5f), navigationNodes[i * rows + (j + 1)]));
                }
                if (i > 0 && j < rows - 1) //top-right diagonal node
                {
                    node.neighbors.Add((new Vector2(22.5f, 67.5f), navigationNodes[(i - 1) * rows + (j + 1)]));
                }
                if (i < columns - 1) //up node
                {
                    node.neighbors.Add((new Vector2(67.5f, 112.5f), navigationNodes[(i + 1) * rows + j]));
                }
                if (i > 0 && j > 0) //top-left diagonal node
                {
                    node.neighbors.Add((new Vector2(112.5f, 157.5f), navigationNodes[(i - 1) * rows + (j - 1)]));
                }
                if (j > 0) //left node
                {
                    node.neighbors.Add((new Vector2(157.5f, 202.5f), navigationNodes[i * rows + (j - 1)]));
                }
                if (i < columns - 1 && j > 0) //bottom-left diagonal node
                {
                    node.neighbors.Add((new Vector2(202.5f, 247.5f), navigationNodes[(i + 1) * rows + (j - 1)]));
                }
                if (i > 0) //down node
                {
                    node.neighbors.Add((new Vector2(247.5f, 292.5f), navigationNodes[(i - 1) * rows + j]));
                }
                if (i < columns - 1 && j < rows - 1) //bottom-right diagonal node
                {
                    node.neighbors.Add((new Vector2(292.5f, 337.5f), navigationNodes[(i + 1) * rows + (j + 1)]));
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
                node.movementCost = (int)NAVIGATIONCOSTS.SOLID;
            }
            else if(neighborIsGround(node)){
                node.movementCost = (int)NAVIGATIONCOSTS.GROUND;
            }
            else
            {
                node.movementCost = (int)NAVIGATIONCOSTS.AIR; //node is air, tougher to navigate than ground, but passable
            }
        }
        navigationMeshGenerationCoroutine = null;
        yield return null;
    }

    /// <summary>
    /// Checks neighbors of the node and returns true if the neighbor that gravity points to is solid
    /// </summary>
    private static bool neighborIsGround(NavigationNode node)
    {
        float minDistance = float.MaxValue;
        float closestDirection = -1.0f;

        for (int i = 0; i < gravityPoints.Count; i++)
        {
            Vector2 gpPos = new Vector2(gravityPoints[i].x, gravityPoints[i].y);
            float distance = Vector2.Distance(node.position, gpPos) / gravityPoints[i].fieldSize;
            float direction = 0;
            if(node.gravityDirection != Vector2.zero)
            {
                direction = Vector2.Angle(Vector2.right, node.gravityDirection.normalized);
            }
            else
            {
                direction = Vector2.Angle(Vector2.right, (gpPos - node.position).normalized);
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = direction;
            }
        }

        foreach (var neighborTuple in node.neighbors)
        {
            if (closestDirection >= neighborTuple.Item1.x && closestDirection < neighborTuple.Item1.y)
            {
                NavigationNode groundNeighbor = neighborTuple.Item2;
                if (groundNeighbor.colliders.Length > 0)
                {
                    return true;
                }
            }
        }
        return false;
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
        const float impassableCost = 10000f;

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
                if (instance.pathDebug)
                {
                    foreach (Vector2 nodePosition in path)
                    {
                        GameObject pathNode = new GameObject("Path Node");
                        pathNode.transform.position = nodePosition;
                        SpriteRenderer sr = pathNode.AddComponent<SpriteRenderer>();
                        sr.sprite = instance.nodeSprite;

                    }
                }

                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            // Evaluate neighboring nodes.
            foreach (var neighborTuple in current.neighbors)
            {
                NavigationNode neighbor = neighborTuple.Item2;
                if (closedSet.Contains(neighbor))
                    continue;

                // Treat extremely expensive nodes as impassable.
                if (neighbor.movementCost >= impassableCost)
                    continue;

                // Calculate the cost of reaching this neighbor.
                float tentativeGScore =
                    gScore[current] +
                    Vector2.Distance(current.position, neighbor.position)
                    * neighbor.movementCost;

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
}
