using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class AStarSearch
{

    // Note: a generic version of A* would abstract over Location and
    // also Heuristic
    public static float Heuristic(Tile a, Tile b)
    {
        return Math.Abs(a.gridPosition.x - b.gridPosition.x) + Math.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    // selectable means that only selectable tiles will be added (for walking within the selectable tile zone)
    //playerTargeting means that the only occupied tile allowed will be the goal tile (to allow targeting of units)
    public static void GeneratePath(Map map, Tile start, Tile goal, bool selectable = false, bool playerTargeting = false)
    { 
        var frontier = new PriorityQueue<TileDistancePair>();

        map.InitPathFinding(start);
        start.parent = null;
                
        frontier.Enqueue(new TileDistancePair(0, start));

        while (frontier.Count() > 0)
        {
            TileDistancePair temp = frontier.Dequeue();
            Tile current = temp.t;
            if (current.Equals(goal))
            {
                break;
            }

            foreach (Tile neighbour in current.adjacencyList)
            {
                int newEstimate = current.distance + neighbour.movementCost;

                if (neighbour.walkable && neighbour.distance > newEstimate && (!neighbour.occupied || (playerTargeting && neighbour == goal)) )
                {
                    // check if neighbour is selectable, if 'selectable' setting is activated
                    if (selectable && !neighbour.selectable)
                    {
                        continue;
                    }

                    neighbour.distance = newEstimate;
                    neighbour.parent = current;
                    float priority = newEstimate + Heuristic(neighbour, goal);
                    frontier.Enqueue(new TileDistancePair(priority, neighbour));
                }
            }
        }
    }

    public static Tile GeneratePathToNearestTarget(Map map, Tile start, List<Tile> targets, bool selectable = false, bool playerTargeting = false)
    {
        // use distance to determine closest player
        int minDistance = int.MaxValue;
        Tile targetTile = targets[0];
        foreach (Tile target in targets)
        {
            AStarSearch.GeneratePath(map, start, target, selectable, playerTargeting);
            if (target.distance < minDistance)
            {
                minDistance = target.distance;
                targetTile = target;
            }
        }

        AStarSearch.GeneratePath(map, start, targetTile, selectable, playerTargeting);
        return targetTile;
    }

}
