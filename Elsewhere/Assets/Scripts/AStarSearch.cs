using UnityEngine;
using UnityEditor;
using System;

public class AStarSearch
{

    // Note: a generic version of A* would abstract over Location and
    // also Heuristic
    public static float Heuristic(Tile a, Tile b)
    {
        return Math.Abs(a.gridPosition.x - b.gridPosition.x) + Math.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    public static void GeneratePath(Map map, Tile start, Tile goal, bool selectable = false)
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

                if (neighbour.walkable && !neighbour.occupied && neighbour.distance > newEstimate)
                {
                    // check if neighbour is selectable if setting is activated
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
}
