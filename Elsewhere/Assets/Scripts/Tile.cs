using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tile : MonoBehaviour, ITile
{
    public bool target { get; set; }
    public bool selectable { get; set; }
    public bool walkable { get; set; }
    public bool attackable { get; set; }
    public bool hasPlayer { get; set; }
    public int movementCost { get; set; }
    public bool occupied { get; set; }

    private bool hover;

    public Tile parent { get; set; }
    public int distance { get; set; }

    private List<Tile> _adjacencyList = new List<Tile>();
    public List<Tile> adjacencyList { get { return _adjacencyList; } set { _adjacencyList = value; } }

    public Vector2Int _gridPosition = Vector2Int.zero;
    public Vector2Int gridPosition { get { return _gridPosition; } set { _gridPosition = value; } }

    // Update is called once per frame
    void Update()
    {
        /*
        if (hover && walkable)
        {
            GetComponent<Renderer>().material.color = new Color(0.43f, 0.76f, 0.86f, 0.3f);
        }*/
        if (attackable)
        {
            GetComponent<Renderer>().material.color = new Color(0.65f, 0.17f, 0.17f, 0.3f);
        }
        else if (hasPlayer)
        {
            GetComponent<Renderer>().material.color = new Color(1, 1, 0, 0.3f);
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = new Color(0, 0.8f, 0.8f, 0.3f);
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
        }
        else
        {
            // transparent nothing
            GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
        }
    }


    // TODO move this to another layer perhaps?
    void OnMouseEnter()
    {
        hover = true;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
        hover = false;
    }

    void OnMouseButtonDown()
    {

    }

    // every turn the tile variables need to be reset
    public void Reset()
    {
        adjacencyList.Clear();

        target = false;
        selectable = false;
        attackable = false;
        //occupied = false; Needs to be manually done if not will cause problems with pathfinding.
        hasPlayer = false;

        parent = null;
        distance = int.MaxValue;
    }

    public void FindNeighbours(List<List<Tile>> tileList, bool includeDiagonals)
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

        /*
        CheckTile(Vector3.left);
        CheckTile(Vector3.right);
        CheckTile(Vector3.up);
        CheckTile(Vector3.down);
        */
    }

    public void CheckTile(Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.25f, 0.01f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                adjacencyList.Add(tile);
            }
        }
    }


    public void OnMouseDown()
    {
        Debug.Log("Tile clicked");
        GameAssets.MyInstance.turnScheduler.OnClickCheckForValidTarget(this);
    }

}


    /*
    public int CompareTo(Tile other)
    {
        if (this.walkable && other.walkable)
        {
            return this.movementCost - other.movementCost;
        }
        else if (this.walkable && !other.walkable)
        {
            return -1;
        } 
        else if (!this.walkable && other.walkable)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    */

