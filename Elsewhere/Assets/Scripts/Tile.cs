using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isStartPoint = false;
    public bool target = false;
    public bool selectable = false;
    public bool walkable = true;
    public bool attackable = false;
    public bool hover = false;
    public int movementCost;

    public bool occupied = false;
    

    // For BFS
    public bool visited = false;
    public Tile parent = null;
    public int distance = int.MaxValue;

    public List<Tile> adjacencyList = new List<Tile>();

    public Vector2Int gridPosition = Vector2Int.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {         
        if (hover && walkable)
        {
            GetComponent<Renderer>().material.color = new Color(0.43f, 0.76f, 0.86f, 0.3f);
        }
        else if (isStartPoint)
        {
            // Magenta
            GetComponent<Renderer>().material.color = new Color(1, 1, 0, 0.2f);
        }
        else if (attackable)
        {
            GetComponent<Renderer>().material.color = new Color(0.65f, 0.17f, 0.17f, 0.3f);
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
        //Debug.Log("Distance: " + distance);
        //Debug.Log("Walkable: " + walkable + ", movementCost: " + movementCost);
        //transform.renderer.material

        //Debug.Log("Current position: " + gridPosition.x + ", " + gridPosition.y);
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

        isStartPoint = false;
        target = false;
        selectable = false;
        attackable = false;

        // For BFS
        visited = false;
        parent = null;
        distance = int.MaxValue;

        // distance = 0; BFS NOT IN USE
    }

    public void FindNeighbours()
    {
        //Reset();
        adjacencyList.Clear();

        CheckTile(Vector3.left);
        CheckTile(Vector3.right);
        CheckTile(Vector3.up);
        CheckTile(Vector3.down);
    }

    public void CheckTile(Vector3 direction)
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.25f, 0.01f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            //Debug.Log("Processing " + tile.gridPosition.x + tile.gridPosition.y);
            if (tile != null)// && tile.walkable && !tile.occupied)
            {
                adjacencyList.Add(tile);

                /* Not working for some reason
                Vector2 origin = new Vector2(tile.transform.position.x, tile.transform.position.y);
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("stopsMovement"));
                if (hit.collider != null && hit.collider.CompareTag("enemy") || hit.collider != null && hit.collider.CompareTag("player"))
                { 
                    {
                        Debug.Log("Found character on tile");
                        //adjacencyList.Add(tile);
                    }
                }
                */
            }
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

