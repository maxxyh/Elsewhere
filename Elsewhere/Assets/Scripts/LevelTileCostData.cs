using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class LevelTileCostData : MonoBehaviour
{

    [SerializeField]
    public Tilemap[] obstacles;

    [SerializeField]
    public Tilemap[] movementCost1;

    [SerializeField]
    public Tilemap[] movementCost2;

    [SerializeField]
    public Tilemap[] movementCost3;

    [SerializeField]
    public Tilemap[] movementCost4;

    private Dictionary<int, Tilemap[]> reference = new Dictionary<int, Tilemap[]>();

    private void Start()
    {
        if (reference.Count == 0)
        {
            reference.Add(0, obstacles);
            reference.Add(1, movementCost4);
            reference.Add(2, movementCost3);
            reference.Add(3, movementCost2);
            reference.Add(4, movementCost1);
        }
    }

    public int GetTileCost(Vector3 position)
    {
        for (int i = 0; i < 5; i++) 
        {
            foreach (Tilemap tilemap in reference[i])
            {
                if (tilemap.GetTile(Vector3Int.RoundToInt(position)) != null)
                {
                    return i == 0 ? 0 : 5 - i;
                }
            }
        }
        return -1;
    }

    public bool IsObstacle(Vector3 position)
    {
        foreach (Tilemap tilemap in obstacles)
        {
            if (tilemap.GetTile(Vector3Int.RoundToInt(position)) != null)
            {
                return true;
            }
        }

        return false;
    }

}
