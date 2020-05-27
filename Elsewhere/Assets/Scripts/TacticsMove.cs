using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool takingTurn = false;

    HashSet<Tile> selectableTiles = new HashSet<Tile>();
    List<Tile> tiles = new List<Tile>();
    Map map;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;

    public bool moving = false;
    public int movementRange;
    public float moveSpeed;
    // TODO think about jumpheight & movement cost

    Vector3 heading = new Vector3();
    Vector3 velocity = new Vector3();

    protected void Init()
    {
        map =  GameObject.FindObjectOfType<Map>();
        foreach (List<Tile> row in map.tileList)
        {   
            foreach (Tile tile in row)
            {
                tiles.Add(tile);
            }
        }
        TurnManager.AddUnit(this);
    }

    public Tile GetCurrentTile()
    {
        return GetTargetTile(gameObject);
    }


    // pick a tile that a player/enemy object is sitting on
    public Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;
        if (Physics.Raycast(target.transform.position + Vector3.back, Vector3.forward, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("stopMovement")));
        {
            //Debug.Log("Found the current tile");
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    public void ComputeAdjacencyList()
    {
        foreach(Tile tile in tiles)
        {
            tile.FindNeighbours();
        }
            
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList(); // working
        currentTile = GetCurrentTile(); // working
        currentTile.current = true;
        currentTile.occupied = true;

        // init Dijkstra
        PriorityQueue<TileDistancePair> processing = new PriorityQueue<TileDistancePair>();
        foreach(Tile tile in tiles)
        {
            tile.distance = int.MaxValue;
        }
        currentTile.distance = 0;
        currentTile.selectable = true;
        
        processing.Enqueue(new TileDistancePair(0, currentTile));


        // relax edges with minimum SP estimate
        while (processing.Count() > 0)
        {
            TileDistancePair temp = processing.Dequeue();
            int distanceEstimate = temp.d;
            Tile node = temp.t;
            if (distanceEstimate == node.distance)
            {
                foreach (Tile neighbour in node.adjacencyList)
                {
                    int newEstimate = node.distance + neighbour.movementCost;
                    
                    if (neighbour.walkable && neighbour.distance > newEstimate && newEstimate <= movementRange)
                    {
                        neighbour.selectable = true;
                        // TODO remove selectableTiles
                        selectableTiles.Add(neighbour);
                        neighbour.distance = newEstimate;
                        neighbour.parent = node;
                        processing.Enqueue(new TileDistancePair(newEstimate, neighbour));
                    }
                }
            }
        }
        
    }

    public void UpdatePosition()
    {
        Tile current = GetCurrentTile();
        current.occupied = true;
    }

    // Generates the path to the tile
    public void GeneratePathToTile(Tile target)
    {
        path.Clear();
        target.target = true;
        moving = true;

        Tile next = target;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    // Actually move to the tile
    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            // check if not reached yet
            if (Vector3.Distance(transform.position, target) >= 0.02f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();

                //transform.up = heading;
                // TODO use or REMOVE heading. doesn't seem to be required yet.
                transform.position += velocity * Time.deltaTime;
                // TODO character not moving to desired location.
            }
            else
            {
                // Tile center reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            currentTile.occupied = false;
            RemoveSelectedTiles();
            moving = false;
            TurnManager.EndTurn();
            // TODO only EndTurn after taking an action e.g. attack, wait, defend etc.
        }
    }

    protected void RemoveSelectedTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile t in selectableTiles)
        {
            t.Reset();
        }
        selectableTiles.Clear();
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        // make into unit vector
        heading.Normalize();
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    // TODO these helper functions can be used for more processing
    public void BeginTurn()
    {
        takingTurn = true;
    }

    public void EndTurn()
    {
        takingTurn = false;
    }

}

class TileDistancePair : IComparable<TileDistancePair>
{

    public int d;
    public Tile t;

    public TileDistancePair(int d, Tile t)
    {
        this.t = t;
        this.d = d;
    }

    public int CompareTo(TileDistancePair other)
    {
        return this.d - other.d;
    }
}