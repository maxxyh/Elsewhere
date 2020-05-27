//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public GameObject TilePrefab;
    [SerializeField] public Tilemap plants;
    [SerializeField] public Tilemap obstacles;
    public int mapSize = 8;

    public List<List<Tile>> tileList = new List<List<Tile>>();
    /*
    public JObject temp = JObject.Parse(File.ReadAllText(@"Config.json"));
    public Dictionary<string, int> movementCostList = JsonConvert.DeserializeObject<Dictionary<string, int>>(temp.ToString());
    */

    // Start is called before the first frame update
    void Start()
    {
        generateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateMap()
    {
        
        for (int i = 0; i < mapSize; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < mapSize; j++)
            {
                GameObject go = Instantiate(TilePrefab, new Vector3(i - mapSize / 2, j - mapSize / 2 + 1, 0),
                    Quaternion.identity);
                go.transform.parent = gameObject.transform;
                Tile tile = go.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(i, j);

                int movementCost = 1;
                bool walkable = true;

                Vector2 origin = new Vector2(tile.transform.position.x, tile.transform.position.y);
                Vector3 hitPoint = new Vector3(tile.transform.position.x, tile.transform.position.y, 0); 
                if (plants.GetTile(Vector3Int.RoundToInt(hitPoint)) != null)
                {
                    movementCost = 2;
                }
                if (obstacles.GetTile(Vector3Int.RoundToInt(hitPoint)) != null)
                {
                    movementCost = -1;
                    walkable = false;
                }


                /* 
                int layerMask = LayerMask.GetMask("plants");
                if (Physics2D.OverlapCircleAll(origin, 0.45f, layerMask).Length > 0)
                {
                    Debug.Log("position " + origin.x + ", " + origin.y + " hit plants");
                    movementCost = 2;
                }
                layerMask = LayerMask.GetMask("stopsMovement");
                if (Physics2D.OverlapBoxAll(origin, new Vector2(0.5f, 0.5f), 0, layerMask).Length > 0)
                {
                    Debug.Log("hit obstacles");
                    movementCost = (int)1e9;
                    walkable = false;
                }

                if (Physics2D.RaycastAll(origin, Vector2.left, 0.5f, layerMask).Length > 0)
                {
                    Debug.Log("hit obstacles");
                    movementCost = (int)1e9;
                    walkable = false;
                }


                
                Vector3 origin = tile.transform.position;
                int layerMask = LayerMask.GetMask("plants");
                RaycastHit hit;
                //Debug.Log("Transform: " + origin.x + ", " + origin.y + ", " + origin.z);

                if (Physics.Raycast(origin, Vector3.forward, Mathf.Infinity, layerMask) )
                {
                    Debug.Log("position " + origin.x + ", " + origin.y + " hit plants");
                    movementCost = 2;
                } 
                else if (Physics.Raycast(origin, Vector3.back, Mathf.Infinity, (1 << 8)))
                {
                    //Debug.Log("hit obstacles");
                    movementCost = (int) 1e9;
                    walkable = false;   
                }
                */
                tile.movementCost = movementCost;
                tile.walkable = walkable;
                
                row.Add(tile);
            }
            tileList.Add(row);
        }
    }
    void OnMouseClick()
    {

    }
}
