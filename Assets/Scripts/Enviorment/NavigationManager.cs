using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private bool runGenerateNavigationMesh = false;
    [SerializeField] private Vector2 meshCenter = Vector2.zero;
    [SerializeField] private Vector2 navigationMeshSize = new Vector2(50, 50);
    [SerializeField] private float navigationMeshResolution = 1f;
    [SerializeField] private LayerMask obstacleLayerMask;
    private Coroutine navigationMeshGenerationCoroutine;
    private static List<NavigationNode> navigationNodes = new List<NavigationNode>();

    public static NavigationManager instance { get; private set; }

    private void Awake()
    {
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
        }
    }

    private IEnumerator generateNavigationMesh()
    {
        navigationNodes.Clear();

        int rows = Mathf.Max(1, Mathf.CeilToInt(navigationMeshSize.y / navigationMeshResolution));
        int columns = Mathf.Max(1, Mathf.CeilToInt(navigationMeshSize.x / navigationMeshResolution));

        float startX = -navigationMeshSize.x / 2f;
        float startY = -navigationMeshSize.y / 2f;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector2 position = meshCenter + new Vector2(
                    startX + (i * navigationMeshResolution),
                    startY + (j * navigationMeshResolution)
                );

                Collider2D[] colliders = Physics2D.OverlapBoxAll(
                    position,
                    new Vector2(4.5f, 4.5f),
                    0f,
                    obstacleLayerMask
                );

                bool isWalkable = colliders.Length == 0;
                /*foreach (Collider2D collider in colliders) //TODO: is walkable based upon what is in the collider
                {

                }*/

                NavigationNode node = new NavigationNode();
                node.position = position;
                node.movementCost = isWalkable ? 1f : 1000f;
                navigationNodes.Add(node);
            }
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                NavigationNode node = navigationNodes[i * rows + j];
                if (j > 0) //left node
                {
                    node.neighbors.Add(navigationNodes[i * rows + (j - 1)]);
                }
                if (j < rows - 1) //right node
                {
                    node.neighbors.Add(navigationNodes[i * rows + (j + 1)]);
                }
                if (i > 0) //down node
                {
                    node.neighbors.Add(navigationNodes[(i - 1) * rows + j]);
                }
                if (i < columns - 1) //up node
                {
                    node.neighbors.Add(navigationNodes[(i + 1) * rows + j]);
                }
            }
        }

        navigationMeshGenerationCoroutine = null;
        yield return null;
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
                return reconstructPath(cameFrom, current, startNode);

            openSet.Remove(current);
            closedSet.Add(current);

            // Evaluate neighboring nodes.
            foreach (NavigationNode neighbor in current.neighbors)
            {
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
