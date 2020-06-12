//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditorInternal;
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
    public HashSet<Tile> GetAttackableTiles()
    {
        return this.attackableTiles;
    }

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

    public void ComputeAdjacencyList(bool includeDiagonals)
    {
        foreach (List<Tile> row in tileList)
        {
            foreach (Tile tile in row)
            {
                tile.FindNeighbours(tileList, includeDiagonals);
            }
        }

    }

    // Finds selectable tiles and updates currentTile as current, occupied
    public void FindSelectableTiles(Tile startTile, float movementRange)
    {
        // if selectable tiles were not removed do not recompute.
        if (selectableTiles.Count > 0)
        {
            foreach (Tile tile in selectableTiles)
            {
                tile.selectable = true;
            }
            return;
        }    
        
        // init Dijkstra
        PriorityQueue<TileDistancePair> processing = new PriorityQueue<TileDistancePair>();

        
        // Compute Adjacency List and reset all distances.
        InitPathFinding(startTile);

        startTile.selectable = true;
        selectableTiles.Add(startTile);

        processing.Enqueue(new TileDistancePair(0, startTile));


        // relax edges with minimum SP estimate
        while (processing.Count() > 0)
        {
            TileDistancePair temp = processing.Dequeue();
            float distanceEstimate = temp.d;
            Tile node = temp.t;
            if ((int) Math.Round(distanceEstimate) == node.distance)
            {
                foreach (Tile neighbour in node.adjacencyList)
                {
                    int newEstimate = node.distance + neighbour.movementCost;

                    if (neighbour.walkable && !neighbour.occupied && neighbour.distance > newEstimate && newEstimate <= movementRange)
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

    // To be called whenever a pathfinding event is done. Re-computes adjacency list and resets tile distances.
    public void InitPathFinding(Tile startTile, bool includeDiagonals = false)
    {
        // Initialise AdjacencyList again to account for updates to tiles e.g. destroyed 
        ComputeAdjacencyList(includeDiagonals);

        foreach (List<Tile> row in tileList)
        {
            foreach (Tile tile in row)
            {
                tile.distance = int.MaxValue;
            }
        }
        startTile.distance = 0;
    }

    public void RemoveSelectableTiles(Tile currentTile, bool destructive = true)
    {

        foreach (Tile t in selectableTiles)
        {
            t.Reset();
        }
        if (destructive)
        {
            selectableTiles.Clear();
        }
    }

    public void FindAttackableTiles(Tile startTile, float attackRange, TargetingStyle targetingStyle = TargetingStyle.SINGLE)
    {
        #region Basic Targeting
        if (targetingStyle == TargetingStyle.SINGLE || targetingStyle == TargetingStyle.MULTI || targetingStyle == TargetingStyle.SELFSINGLE)
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
        #endregion

        #region Self Targeting
        if (targetingStyle == TargetingStyle.SELF || targetingStyle == TargetingStyle.SELFSINGLE)
        {
            startTile.attackable = true;
            attackableTiles.Add(startTile);
        }
        #endregion

        #region Radius Targeting
        if (targetingStyle == TargetingStyle.RADIUS)
        { 
            // init Dijkstra
            PriorityQueue<TileDistancePair> processing = new PriorityQueue<TileDistancePair>();


            // Compute Adjacency List and reset all distances.
            InitPathFinding(startTile, true);

            attackableTiles.Add(startTile);

            processing.Enqueue(new TileDistancePair(0, startTile));

            // relax edges with minimum SP estimate
            while (processing.Count() > 0)
            {
                TileDistancePair temp = processing.Dequeue();
                float distanceEstimate = temp.d;
                Tile node = temp.t;
                if ((int)Math.Round(distanceEstimate) == node.distance)
                {
                    foreach (Tile neighbour in node.adjacencyList)
                    {
                        int newEstimate = node.distance + 1;

                        if (neighbour.walkable && neighbour.distance > newEstimate && newEstimate <= attackRange)
                        {
                            neighbour.attackable= true;
                            attackableTiles.Add(neighbour);
                            neighbour.distance = newEstimate;
                            processing.Enqueue(new TileDistancePair(newEstimate, neighbour));
                        }
                    }
                }
            }
        }    
        #endregion
        
    }

    public bool PlayerTargetInRange(Tile startTile, float attackRange, Unit player)
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
                    Tile newTile = tileList[newX][newY];
                    if (newTile.walkable && passable[j])
                    {
                        if (newTile == player.currentTile)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        passable[j] = false;
                    }
                }
            }
        }
        return false;
    }

    public void RemoveAttackableTiles()
    {
        foreach(Tile t in attackableTiles)
        {
            t.attackable = false;
        }
        attackableTiles.Clear();
    }
}


class TileDistancePair : IComparable<TileDistancePair>
{

    public float d;
    public Tile t;

    public TileDistancePair(float d, Tile t)
    {
        this.t = t;
        this.d = d;
    }

    public int CompareTo(TileDistancePair other)
    {
        if (this.d > other.d)
        {
            return 1;
        }
        else if (other.d > this.d)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}

