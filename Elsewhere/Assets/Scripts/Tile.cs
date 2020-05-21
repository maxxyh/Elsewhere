using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool walkable = true;

    // For BFS
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    public List<Tile> adjacencyList = new List<Tile>();

    public Vector2 gridPosition = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (current)
        {
            // Magenta
            GetComponent<Renderer>().material.color = new Color(1,1,0,0.2f);
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = new Color(0,0.8f,0.8f,0.3f);
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = new Color(0,1,0,0.3f);
        }
        else
        {
            // transparent nothing
            GetComponent<Renderer>().material.color = new Color(1f,1f,1f,0f);
        }
    }


    // TODO move this to another layer perhaps?
    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = new Color(0.43f,0.76f,0.86f,0.3f);
        //transform.renderer.material

        //Debug.Log("Current position: " + gridPosition.x + ", " + gridPosition.y);
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = new Color(1f,1f,1f,0f);
    }

    void OnMouseButtonDown()
    {

    }

    // every turn the tile variables need to be reset
    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;
        walkable = true;

        // For BFS
        visited = false;
        parent = null;
        distance = 0;
    }

    public void FindNeighbours()
    {
        Reset();

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
            if (!(tile == null) && tile.walkable)
            {
                adjacencyList.Add(tile);

                // REVIEW does not work because of tilemap
                /*
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position + Vector3.back, Vector3.forward, out hit, 2))
                {
                    Debug.Log("Found adjacent tile");
                    adjacencyList.Add(tile);
                }
                */
            }
        }
    }
}
