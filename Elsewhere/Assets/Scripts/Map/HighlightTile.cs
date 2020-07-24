using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightTile : MonoBehaviour
{
    public Vector2Int gridPosition = Vector2Int.zero;

    public List<HighlightTile> adjacencyList = new List<HighlightTile>();
    public int distance = int.MaxValue;
    public HighlightMap highlightMap;
    public bool hover = false;

    private void Update()
    {
        if (hover)
        {
            GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.3f);
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0);
        }

    }

    public void FindNeighbours(List<List<HighlightTile>> tileList, bool includeDiagonals)
    {
        adjacencyList.Clear();

        int currX = this.gridPosition.x, currY = this.gridPosition.y;

        List<int> hor, vert;

        if (includeDiagonals)
        {
            hor = new List<int>() { -1, 0, 1, 0, -1, 1, -1, 1 };
            vert = new List<int>() { 0, 1, 0, -1, 1, 1, -1, -1 };
        }
        else
        {
            hor = new List<int>() { -1, 0, 1, 0 };
            vert = new List<int>() { 0, 1, 0, -1 };
        }


        for (int j = 0; j < hor.Count; j++)
        {
            int newX = currX + hor[j];
            int newY = currY + vert[j];

            // check that is valid tile
            if (newX >= 0 && newX < tileList.Count && newY >= 0 && newY < tileList[0].Count)
            {
                this.adjacencyList.Add(tileList[newX][newY]);
            }
        }
    }

    public void Reset()
    {
        distance = int.MaxValue;
    }

    public void MouseOver()
    {
        if (GameAssets.MyInstance.turnScheduler.currTurn == Team.PLAYER)
        {
            highlightMap.HighlightSelectedTiles(this);
        }
    }

}
