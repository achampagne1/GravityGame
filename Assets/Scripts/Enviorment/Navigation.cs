using System.Collections.Generic;
using UnityEngine;

public enum NAVIGATIONCOSTS
{
    SOLID = 1000,
    AIR = 10,
    GROUND = 1,
}

public class NavigationNode
{
    public Vector2 position;
    public Vector2 gravityDirection = Vector2.zero;
    public int movementCost = 1;
    public List<(Vector2,NavigationNode)> neighbors = new List<(Vector2, NavigationNode)>();
    public Collider2D[] colliders;
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