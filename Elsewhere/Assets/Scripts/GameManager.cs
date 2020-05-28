using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject PlayerPrefab;
    public int mapSize = 8;
    List<List<Tile>> map;
    List<PlayerMovement> players;

    public int currentPlayerIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        //generateMap();
        generatePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        //players[currentPlayerIndex].ExecuteTurn();
    }

    public void nextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex +1) % (players.Count -1);
    }

    void generateMap()
    {
        map = new List<List<Tile>>();
        for (int i = 0 ; i < mapSize ; i ++) {
            List<Tile> row = new List<Tile>();
            for (int j = 0 ; j < mapSize ; j++) {
                Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3(i - mapSize/2 + 1, j - mapSize/2 + 1, 0),
                    Quaternion.Euler(new Vector3()) )).GetComponent<Tile>();
                tile.gridPosition = new Vector2(i,j);
                row.Add(tile);
            }
            map.Add(row);
        }
    }

    void generatePlayers() {
        players = new List<PlayerMovement>();
        PlayerMovement player = ((GameObject)Instantiate(PlayerPrefab, new Vector3(0,0,0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerMovement>();
        player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player.tag = "player";
        players.Add(player);

        player = ((GameObject)Instantiate(PlayerPrefab, new Vector3(-mapSize/2+1,-mapSize/2+1,0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerMovement>();
        player.gridPosition = new Vector2(0,0);
        player.tag = "enemy";
        players.Add(player);
    }
}
