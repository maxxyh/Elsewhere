using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private int hoverInt;

    private Unit toDisplay;

    public Tile parent { get; set; }
    public int distance { get; set; }

    //private Material material { get; set; }

    private SpriteRenderer spriteRenderer;

    private List<Tile> _adjacencyList = new List<Tile>();
    public List<Tile> adjacencyList { get { return _adjacencyList; } set { _adjacencyList = value; } }

    public Vector2Int _gridPosition = Vector2Int.zero;
    public Vector2Int gridPosition { get { return _gridPosition; } set { _gridPosition = value; } }

    private void Update()
    {
        if (hover)
        {
            hoverInt--;
            if (hoverInt < 0)
            {
                MouseExit();
            }
        }

        if (attackable)
        {
            spriteRenderer.color = new Color(0.65f, 0.17f, 0.17f, 0.3f);
        }
        else if (hasPlayer)
        {
            spriteRenderer.color = new Color(1, 1, 0, 0.3f);
        }
        else if (target)
        {
            spriteRenderer.color = new Color(0, 0.8f, 0.8f, 0.3f);
        }
        else if (selectable)
        {
            spriteRenderer.color = new Color(0, 1, 0, 0.3f);
        }
        else
        {
            // transparent nothing
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }
    }
    
    /*private void Update()
    {
        /*
        if (hover && walkable)
        {
            GetComponent<Renderer>().material.color = new Color(0.43f, 0.76f, 0.86f, 0.3f);
        }#1#
        if (attackable)
        {
            material.color = new Color(0.65f, 0.17f, 0.17f, 0.3f);
        }
        else if (hasPlayer)
        {
            material.color = new Color(1, 1, 0, 0.3f);
        }
        else if (target)
        {
            material.color = new Color(0, 0.8f, 0.8f, 0.3f);
        }
        else if (selectable)
        {
            material.color = new Color(0, 1, 0, 0.3f);
        }
        else
        {
            // transparent nothing
            material.color = new Color(1f, 1f, 1f, 0f);
        }
    }*/
    
    

    private void Awake()
    {
        //material = GetComponent<Renderer>().material;
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    /*public void OnMouseEnter()
    {
        if (occupied)
        {
            toDisplay = GameAssets.MyInstance.turnScheduler.players.Find(x => x.currentTile == this);
            if (toDisplay == null)
            {
                toDisplay = GameAssets.MyInstance.turnScheduler.enemies.Find(x => x.currentTile == this);
            }
            if (toDisplay != null)
            {
                toDisplay.SetStatPanelActive();
            }
        }
    }*/

    public void MouseOver()
    {
        hover = true;
        hoverInt = 1;
        if (occupied)
        {
            if (toDisplay == null)
            {
                toDisplay = GameAssets.MyInstance.turnScheduler.players.Find(x => x.currentTile == this);
                if (toDisplay == null)
                {
                    toDisplay = GameAssets.MyInstance.turnScheduler.enemies.Find(x => x.currentTile == this);
                }
            }

            if (toDisplay != null)
            {
                toDisplay.SetStatPanelActive();
            }
        }
    }
    
    public void MouseExit()
    {
        hover = false;
        if (occupied && toDisplay != null)
        {
            toDisplay.SetStatPanelInActive();
            toDisplay = null;
        }
    }

    public void MouseDown()
    {
        GameAssets.MyInstance.turnScheduler.OnClickCheckForValidTarget(this);
    }

    /*public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (occupied)
            {
                CheckAndActivateMajorStatPanel();
            }
        }
        else if (Input.GetMouseButtonDown(2))
        {
            if (occupied)
            {
                Unit test = GameAssets.MyInstance.turnScheduler.players.Find(x => x.currentTile == this);
                test.OnLevelUp();
            }
        }
    }*/
    public void RightClick()
    {
        if (occupied)
        {
            CheckAndActivateMajorStatPanel();
        }
    }

    public void CheckAndActivateMajorStatPanel()
    {
        toDisplay = GameAssets.MyInstance.turnScheduler.players.Find(x => x.currentTile == this);
        if (toDisplay == null)
        {
            toDisplay = GameAssets.MyInstance.turnScheduler.enemies.Find(x => x.currentTile == this);
        }
        if (toDisplay != null)
        {
            toDisplay.majorStatPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("no unit detected");
        }
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

