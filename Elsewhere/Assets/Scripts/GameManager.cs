using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject[] PlayerPrefabs;
    public GameObject EnemyPrefab;
    public Map map;
    public TurnScheduler turnScheduler;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();


    public Camera worldCamera;

    private Dictionary<string, float> defaultStats = new Dictionary<string, float>();
    private List<Ability> defaultAbilities = new List<Ability>();

    // Start is called before the first frame update
    void Start()
    {
        map.generateMap();
        generatePlayers();
        turnScheduler.Init(players, enemies);
    }

    // Update is called once per frame
    void Update()
    {
        
        //players[currentPlayerIndex].ExecuteTurn();
    }

    // TODO add the EnemyUnits too.
    void generatePlayers() {

        // default stats
        defaultStats.Add("attackDamage", 6);
        defaultStats.Add("magicDamage", 5);
        defaultStats.Add("mana", 20);
        defaultStats.Add("HP", 10);
        defaultStats.Add("armor", 3);
        defaultStats.Add("magicRes", 2);
        defaultStats.Add("movementRange", 4);
        defaultStats.Add("attackRange", 2);

        // default abilities
        defaultAbilities.Add(new AbilityHealingWave());
        defaultAbilities.Add(new AbilityWhirlwindSlash());


        // PLAYERS
        
        PlayerUnit player = ((GameObject)Instantiate(PlayerPrefabs[0], new Vector3(0, 0, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player.tag = "player";
        player.AssignStats(defaultStats);
        player.AssignMap(map);
        player.AssignAbilities(defaultAbilities);
        Debug.Log("Player abilities size " + player.abilities.Count);
        player.UpdateUI();
        players.Add(player);

        PlayerUnit player2 = ((GameObject)Instantiate(PlayerPrefabs[1], new Vector3(0, 2, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player2.tag = "player";
        player2.AssignStats(defaultStats);
        player2.AssignMap(map);
        player2.AssignAbilities(defaultAbilities);
        Debug.Log("Player abilities size " + player.abilities.Count);
        player2.UpdateUI();
        players.Add(player2);



        // ENEMIES

        // TODO DOESN'T ACTUALLY INITIATE A VALID ENEMY
        EnemyUnit enemy = ((GameObject)Instantiate(EnemyPrefab, new Vector3(-map.mapSize / 2 + 1, -map.mapSize / 2 + 1, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy.tag = "enemy";
        enemy.AssignStats(defaultStats);
        enemy.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy);

        EnemyUnit enemy2 = ((GameObject)Instantiate(EnemyPrefab, new Vector3(-1 -map.mapSize / 2 + 1, -map.mapSize / 2 + 1, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy2.tag = "enemy";
        enemy2.AssignStats(defaultStats);
        enemy2.AssignMap(map);
        enemy2.UpdateUI();
        enemies.Add(enemy2);
    }
}
