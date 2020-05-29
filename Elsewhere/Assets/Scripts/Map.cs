﻿//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public GameObject TilePrefab;
    [SerializeField] public Tilemap plants;
    [SerializeField] public Tilemap obstacles;
    public int mapSize = 8;

    public List<List<Tile>> tileList = new List<List<Tile>>();

    // For UI
    HashSet<Tile> selectableTiles = new HashSet<Tile>();
    HashSet<Tile> attackableTiles = new HashSet<Tile>();


    // Start is called before the first frame update
    void Start()
    {
        //generateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateMap()
    {
        
        for (int i = 0; i < mapSize; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < mapSize; j++)
            {
                GameObject go = Instantiate(TilePrefab, new Vector3(i - mapSize / 2, j - mapSize / 2 + 1, 0),
                    Quaternion.identity);
                go.transform.parent = gameObject.transform;
                //go.layer = LayerMask.NameToLayer("map");
                Tile tile = go.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(i, j);

                int movementCost = 1;
                bool walkable = true;

                Vector2 origin = new Vector2(tile.transform.position.x, tile.transform.position.y);
                Vector3 hitPoint = new Vector3(tile.transform.position.x, tile.transform.position.y, 0); 
                if (plants.GetTile(Vector3Int.RoundToInt(hitPoint)) != null)
                {
                    movementCost = 2;
                }
                if (obstacles.GetTile(Vector3Int.RoundToInt(hitPoint)) != null)
                {
                    movementCost = -1;
                    walkable = false;
                }

                tile.movementCost = movementCost;
                tile.walkable = walkable;
                
                row.Add(tile);
            }
            tileList.Add(row);
        }
    }
    public Tile GetCurrentTile(Vector3 currentPos)
    {
        Vector3Int currCoor = Vector3Int.RoundToInt(currentPos);
        Tile current = tileList[currCoor.x + mapSize / 2][currCoor.y + mapSize / 2 - 1];
        
        //Tile current = GetTargetTile(Vector3Int.RoundToInt(currentPos));
        if (current != null)
        {
            current.occupied = true;
        }
        return current;
    }

    // pick a tile that a player/enemy object is sitting on
    public static Tile GetTargetTile(Vector3 targetPosition)
    {
        Tile tile = null;
        if (Physics.Raycast(targetPosition, Vector3.back, out RaycastHit hit, Mathf.Infinity))
        {
            //Debug.Log("Found the current tile");
            tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                Debug.Log("found tile");
            }
        }
        return tile;
    }

    public void ComputeAdjacencyList()
    {
        foreach (List<Tile> row in tileList)
        {
            foreach (Tile tile in row)
            {
                tile.FindNeighbours();
            }
        }

    }

    // Finds selectable tiles and updates currentTile as current, occupied
    public void FindSelectableTiles(Tile startTile, float movementRange)
    {
        // Initialise AdjacencyList again to account for updates to player movement
        ComputeAdjacencyList(); 

        // init Dijkstra
        PriorityQueue<TileDistancePair> processing = new PriorityQueue<TileDistancePair>();
        foreach (List<Tile> row in tileList)
        {
            foreach (Tile tile in row)
            {
                tile.distance = int.MaxValue;
            }
        }
        startTile.distance = 0;
        startTile.selectable = true;
        selectableTiles.Add(startTile);

        processing.Enqueue(new TileDistancePair(0, startTile));


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

    public void RemoveSelectedTiles(Tile currentTile, bool destructive = true)
    {
        currentTile.isStartPoint = false;

        foreach (Tile t in selectableTiles)
        {
            t.Reset();
        }
        if (destructive)
        {
            selectableTiles.Clear();
        }
    }

    public void FindAttackableTiles(Tile startTile, float attackRange)
    {
        int[] hor = { -1, 0, 1, 0 };
        int[] vert = { 0, 1, 0, -1 };

        int currX = startTile.gridPosition.x, currY = startTile.gridPosition.y;

        bool[] passable = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            passable[i] = true;
        }

        for (int i = 1; i <= attackRange; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int newX = currX + i * hor[j];
                int newY = currY + i * vert[j];

                if (newX >= 0 && newX < mapSize && newY >= 0 && newY < mapSize)
                {
                    if (tileList[newX][newY].walkable && passable[j])
                    {
                        tileList[newX][newY].attackable = true;
                        attackableTiles.Add(tileList[newX][newY]);
                    }
                    else
                    {
                        passable[j] = false;
                    }
                }
            }
        }
    }

    public void RemoveAttackableTiles()
    {
        foreach(Tile t in attackableTiles)
        {
            t.attackable = false;
        }
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