using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HighlightMap : MonoBehaviour
{
    [SerializeField] public GameObject highlightTilePrefab;
    public List<List<HighlightTile>> tileList = new List<List<HighlightTile>>();
    [SerializeField] public Map map;
    public List<HighlightTile> selectedTiles;

    public void HighlightSelectedTiles(HighlightTile mainTile)
    {
        // check if tile being hovered is attackable

        Tile mapStartTile = map.tileList[mainTile.gridPosition.x][mainTile.gridPosition.y];
        if (!mapStartTile.attackable)
        {
            return;
        }

        //RemoveSelectedTiles();

        Ability ability = GameAssets.MyInstance.turnScheduler.currUnit?.chosenAbility;
        TargetingStyle targetingStyle = TargetingStyle.SINGLE;
        if (ability != null)
        {
            targetingStyle = ability.targetingStyle;
        }
        
        if (targetingStyle == TargetingStyle.MULTI)
        {
            int multiAbilityRange = (int) ability.multiAbilityRange;
            //Debug.Log("multiAbilityRange = " + multiAbilityRange);

            // init BFS
            Queue<HighlightTile> processing = new Queue<HighlightTile>();

            // Compute Adjacency List and reset all distances.
            InitPathFinding(mainTile);
            processing.Enqueue(mainTile);

            // relax edges with minimum SP estimate
            while (processing.Count > 0)
            {
                HighlightTile node = processing.Dequeue();
                foreach (HighlightTile neighbour in node.adjacencyList)
                {
                    Tile mapEquivalent = map.tileList[node.gridPosition.x][node.gridPosition.y];
                    if (mapEquivalent.walkable && neighbour.distance > node.distance + 1 && node.distance + 1 <= multiAbilityRange)
                    {
                        neighbour.distance = node.distance + 1;
                        selectedTiles.Add(neighbour);
                        processing.Enqueue(neighbour);
                    }
                }
            }
        }
        else if (targetingStyle == TargetingStyle.SINGLE || targetingStyle == TargetingStyle.SELF || targetingStyle == TargetingStyle.SELFSINGLE)
        {
            selectedTiles.Add(mainTile);
        }

        else if (targetingStyle == TargetingStyle.RADIUS)
        {
            return;
        }
        // how to change back though? Tile states?

        foreach (HighlightTile tile in selectedTiles)
        {
            tile.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.3f);
        }
    }

    public void RemoveSelectedTiles()
    {
        foreach(HighlightTile tile in selectedTiles)
        {
            tile.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0);
        }
        selectedTiles.Clear();
    }

    public void generateUIMap()
    {
        int mapSize = map.mapSize;
        for (int i = 0; i < mapSize; i++)
        {
            List<HighlightTile> row = new List<HighlightTile>();
            for (int j = 0; j < mapSize; j++)
            {
                GameObject go = Instantiate(highlightTilePrefab, new Vector3(i - mapSize / 2, j - mapSize / 2 + 1, 0),
                    Quaternion.identity);
                go.transform.parent = gameObject.transform;
                //go.layer = LayerMask.NameToLayer("map");
                HighlightTile tile = go.GetComponent<HighlightTile>();
                tile.highlightMap = this;
                tile.gridPosition = new Vector2Int(i, j);
                tile.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0);

                // set addional settings required

                row.Add(tile);
            }
            tileList.Add(row);
        }
    }

    public void ComputeAdjacencyList(bool includeDiagonals)
    {
        foreach (List<HighlightTile> row in tileList)
        {
            foreach (HighlightTile tile in row)
            {
                tile.FindNeighbours(tileList, includeDiagonals);
            }
        }

    }

    // To be called whenever a pathfinding event is done. Re-computes adjacency list and resets tile distances.
    public void InitPathFinding(HighlightTile startTile, bool includeDiagonals = false)
    {
        // Initialise AdjacencyList again to account for updates to tiles e.g. destroyed 
        ComputeAdjacencyList(includeDiagonals);

        foreach (List<HighlightTile> row in tileList)
        {
            foreach (HighlightTile tile in row)
            {
                tile.distance = int.MaxValue;
            }
        }
        startTile.distance = 0;
    }

    public void SetClicked()
    {
        Debug.Log("selected Tiles size" + selectedTiles.Count);
        foreach (HighlightTile tile in selectedTiles)
        {
            tile.clicked = true;
        }
    }

    public void RemoveClicked()
    {
        foreach (HighlightTile tile in selectedTiles)
        {
            tile.clicked = false;
        }
    }
}
