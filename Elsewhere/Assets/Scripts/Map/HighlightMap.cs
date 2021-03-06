﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class HighlightMap : MonoBehaviour
{
    [SerializeField] public GameObject highlightTilePrefab;
    public List<List<HighlightTile>> tileList = new List<List<HighlightTile>>();
    [SerializeField] public Map map;
    public HashSet<HighlightTile> selectedTiles = new HashSet<HighlightTile>();
    private bool clicked;

    public void HighlightSelectedTiles(HighlightTile mainTile)
    {
        // if alr clicked, do nothing: 
        if (clicked)
        {
            return;
        }
        
        
        RemoveSelectedTiles();

        // check if tile being hovered is attackable

        Tile mapStartTile = map.tileList[mainTile.gridPosition.x][mainTile.gridPosition.y];
        if (!mapStartTile.attackable)
        {
            return;
        }

        Ability ability = GameAssets.MyInstance.turnScheduler.currUnit?.chosenAbility;
        if (ability == null)
        {
            ability = new AbilityDefault();
        }
        TargetingStyle targetingStyle = ability.targetingStyle;

        #region Getting the correct target team
        IEnumerable<Unit> friendlyTeam;
        TurnScheduler turnScheduler = GameAssets.MyInstance.turnScheduler;
        if (turnScheduler.currTurn == Team.ENEMY)
        {
            if (ability.targetsSameTeam)
            {
                friendlyTeam = turnScheduler.enemies;
            }
            else
            {
                friendlyTeam = turnScheduler.players;
            }
        }
        else
        {
            if (ability.targetsSameTeam)
            {
                friendlyTeam = turnScheduler.enemies;
            }
            else
            {
                friendlyTeam = turnScheduler.players;
            }
        }
        #endregion

        bool IsFriendlyFire(Tile mapEquivalent)
        {
            if (mapEquivalent.occupied)
            {
                foreach (Unit unit in friendlyTeam)
                {
                    if (unit.currentTile == mapEquivalent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        if (targetingStyle == TargetingStyle.MULTI)
        {
            

            int multiAbilityRange = ability.multiAbilityRange;
            //Debug.Log("multiAbilityRange = " + multiAbilityRange);

            // init BFS
            Queue<HighlightTile> processing = new Queue<HighlightTile>();

            // Compute Adjacency List and reset all distances.
            InitPathFinding(mainTile);
            selectedTiles.Add(mainTile);
            processing.Enqueue(mainTile);

            // relax edges with minimum SP estimate
            while (processing.Count > 0)
            {
                HighlightTile node = processing.Dequeue();
                foreach (HighlightTile neighbour in node.adjacencyList)
                {
                    Tile mapEquivalent = map.tileList[neighbour.gridPosition.x][neighbour.gridPosition.y];
                    if (mapEquivalent.walkable && neighbour.distance > node.distance + 1 && node.distance + 1 <= multiAbilityRange)
                    {
                        if (!IsFriendlyFire(mapEquivalent))
                        {
                            neighbour.distance = node.distance + 1;
                            selectedTiles.Add(neighbour);
                            processing.Enqueue(neighbour);
                        }
                    }
                }
            }
        }
        else if (targetingStyle == TargetingStyle.SINGLE || targetingStyle == TargetingStyle.SELF || targetingStyle == TargetingStyle.SELFSINGLE)
        {
            Tile mapEquivalent = map.tileList[mainTile.gridPosition.x][mainTile.gridPosition.y];
            if (!IsFriendlyFire(mapEquivalent))
            {
                selectedTiles.Add(mainTile);
            }
        }

        else if (targetingStyle == TargetingStyle.RADIUS)
        {
            foreach(Tile tile in map.GetAttackableTiles())
            {
                if (!IsFriendlyFire(tile))
                {
                    selectedTiles.Add(tileList[tile.gridPosition.x][tile.gridPosition.y]);
                }
            }
        }
        
        else if (targetingStyle == TargetingStyle.OBSTACLES)
        {
            if (map.tileCostReference.IsObstacle(mainTile.transform.position))
            {
                selectedTiles.Add(mainTile);
            }
        }

        foreach (HighlightTile tile in selectedTiles)
        {
            tile.hover = true;
        }
    }

    public void RemoveSelectedTiles()
    {
        foreach(HighlightTile tile in selectedTiles)
        {
            tile.hover = false;
        }
        selectedTiles.Clear();
    }

    public void generateUIMap()
    {
        Vector2Int mapSize = map.mapSize;
        Vector2Int bottomLeft = map.bottomLeft;
        for (int i = 0; i < mapSize.x; i++)
        {
            List<HighlightTile> row = new List<HighlightTile>();
            for (int j = 0; j < mapSize.y; j++)
            {
                GameObject go = Instantiate(highlightTilePrefab, new Vector3(i + bottomLeft.x, j + bottomLeft.y, 0),
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
        clicked = true;
    }

    public void RemoveClicked()
    {
        clicked = false;
        RemoveSelectedTiles();
    }
}
