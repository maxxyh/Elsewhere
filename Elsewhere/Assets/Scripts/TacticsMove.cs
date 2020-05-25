using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool takingTurn = false;

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;

    public bool moving = false;
    public int movementRange = 2;
    public float moveSpeed = 2;
    // TODO think about jumpheight & movement cost

    Vector3 heading = new Vector3();
    Vector3 velocity = new Vector3();

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("tile");

        TurnManager.AddUnit(this);
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
    }


    // pick a tile that a player/enemy object is sitting on
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position + Vector3.back, Vector3.forward, out hit, 2))
        {
            //Debug.Log("Found the current tile");
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    public void ComputeAdjacencyList()
    {
        foreach(GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbours();
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList();
        GetCurrentTile();
        currentTile.current = true;

        Queue<Tile> processing = new Queue<Tile>();

        processing.Enqueue(currentTile);
        currentTile.visited = true;

        while (processing.Count > 0 )
        {
            Tile temp = processing.Dequeue();
            selectableTiles.Add(temp);
            temp.selectable = true;
            if (temp.distance < movementRange)
            {
                foreach (Tile neighbour in temp.adjacencyList)
                {
                    if (!neighbour.visited)
                    {
                        neighbour.visited = true;
                        neighbour.parent = temp;
                        neighbour.distance = temp.distance +1;
                        processing.Enqueue(neighbour);
                    }
                }
            }
        }
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
            RemoveSelectedTiles();
            moving = false;
            TurnManager.EndTurn();
            // TODO only EndTurn after taking an action e.g. attack, wait, defend etc.
        }
    }

    protected void RemoveSelectedTiles()
    {
        if (currentTile != null )
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach(Tile t in selectableTiles)
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
