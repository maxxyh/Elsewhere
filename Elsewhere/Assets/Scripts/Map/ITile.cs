using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    List<Tile> adjacencyList { get; set; }
    bool attackable { get; set; }
    int distance { get; set; }
    Vector2Int gridPosition { get; set; }
    bool hasPlayer { get; set; }
    int movementCost { get; set; }
    bool occupied { get; set; }
    Tile parent { get; set; }
    bool selectable { get; set; }
    bool target { get; set; }
    bool walkable { get; set; }

    void CheckTile(Vector3 direction);
    void FindNeighbours(List<List<Tile>> tileList, bool includeDiagonals);
    void MouseDown();
    void OnMouseExit();
    void OnMouseOver();
    void Reset();
}