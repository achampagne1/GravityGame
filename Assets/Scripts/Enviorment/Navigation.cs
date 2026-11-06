using System.Collections.Generic;
using UnityEngine;

public class NavigationNode
{
    public Vector2 position;
    public float movementCost = 1f;
    public List<NavigationNode> neighbors = new List<NavigationNode>();
}

public class NavigationRules
{
    public bool canMoveTo(NavigationNode from,NavigationNode to,NavigationProfile profile)
    {
        //if (!to.isWalkable)
          //  return false;

        if (!profile.canTraverse(from.position, to.position))
            return false;

        return true;
    }

    float getMovementCost(NavigationNode from,NavigationNode to,NavigationProfile enemy)
    {
        float cost = Vector2.Distance(
            from.position,
            to.position
        );

        cost *= to.movementCost;

        return cost;
    }
}