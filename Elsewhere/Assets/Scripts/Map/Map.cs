//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] public GameObject TilePrefab;
    [SerializeField] public LevelTileCostData tileCostReference;
    [SerializeField] public Vector2Int mapSize;
    [SerializeField] public Vector2Int bottomLeft;

    public List<List<Tile>> tileList = new List<List<Tile>>();

    // For UI
    HashSet<Tile> selectableTiles = new HashSet<Tile>();
    HashSet<Tile> attackableTiles = new HashSet<Tile>();

    public List<Tile> UnitsInSelectableRange = new List<Tile>();

    public HashSet<Tile> GetAttackableTiles()
    {
        return this.attackableTiles;
    }

    public HashSet<Tile> GetSelectableTiles()
    {
        return this.selectableTiles;
    }

    public void GenerateMap()
    {
        
        for (int i = 0; i < mapSize.x; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < mapSize.y; j++)
            {
                GameObject go = Instantiate(TilePrefab, new Vector3(i + bottomLeft.x, j + bottomLeft.y, 0),
                    Quaternion.identity);
                go.transform.parent = gameObject.transform;
                //go.layer = LayerMask.NameToLayer("map");
                Tile tile = go.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(i, j);

                int movementCost;
                bool walkable = true;

                Vector3 hitPoint = new Vector3(tile.transform.position.x, tile.transform.position.y, 0);
                movementCost = tileCostReference.GetTileCost(hitPoint);
                if (movementCost == -1)
                {
                    Debug.LogError("No movement cost as tile does not exist.");
                    movementCost = 0;
                } 
                if (movementCost == 0)
                {
                    movementCost = int.MaxValue;
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
        Tile current = tileList[currCoor.x - bottomLeft.x][currCoor.y - bottomLeft.y];
        
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

                    if (neighbour.walkable && neighbour.distance > newEstimate && newEstimate <= movementRange)
                    {
                        if (!neighbour.occupied)
                        {
                            neighbour.selectable = true;
                            selectableTiles.Add(neighbour);
                        } else
                        {
                            UnitsInSelectableRange.Add(neighbour);
                        }
                           

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
        
        // Initialize AdjacencyList
        // Find tileDistance
        foreach (List<Tile> row in tileList)
        {
            foreach (Tile tile in row)
            {
                tile.FindNeighbours(tileList, includeDiagonals);
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
            UnitsInSelectableRange.Clear();
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

                    if (newX >= 0 && newX < mapSize.x && newY >= 0 && newY < mapSize.y)
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

        #region Obstacle Targeting
        if (targetingStyle == TargetingStyle.OBSTACLES)
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

                    if (newX >= 0 && newX < mapSize.x && newY >= 0 && newY < mapSize.y)
                    {
                        if (!tileList[newX][newY].walkable)
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
        
    }

    // Need to write for all the targeting styles
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

                if (newX >= 0 && newX < mapSize.x && newY >= 0 && newY < mapSize.y)
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
    public void RemoveAllObstaclesFromTile(Tile toBreak)
    {
        Vector3Int toBreakVector = Vector3Int.RoundToInt(toBreak.transform.position);

        foreach (Tilemap tilemap in tileCostReference.obstacles)
        {
            tilemap.SetTile(toBreakVector, null);
        }

        toBreak.walkable = true;
        toBreak.movementCost = tileCostReference.GetTileCost(toBreak.transform.position);
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

