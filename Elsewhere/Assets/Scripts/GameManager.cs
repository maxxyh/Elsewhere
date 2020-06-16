using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject[] PlayerPrefabs;
    public GameObject[] EnemyPrefab;
    public Map map;
    public HighlightMap highlightMap;
    public TurnScheduler turnScheduler;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();


    public Camera worldCamera;

    private Dictionary<StatString, float> defaultStats = new Dictionary<StatString, float>();
    

    // Start is called before the first frame update
    void Start()
    {
        map.generateMap();
        highlightMap.generateUIMap();
        generatePlayers();
        turnScheduler.Init(players, enemies);
    }

    void generatePlayers() {

        // default stats
        defaultStats.Add(StatString.ATTACK_DAMAGE, 6);
        defaultStats.Add(StatString.MAGIC_DAMAGE, 5);
        defaultStats.Add(StatString.MANA, 20);
        defaultStats.Add(StatString.HP, 10);
        defaultStats.Add(StatString.ARMOR, 3);
        defaultStats.Add(StatString.MAGIC_RES, 2);
        defaultStats.Add(StatString.MOVEMENT_RANGE, 4);
        defaultStats.Add(StatString.ATTACK_RANGE, 2);


        // default abilities
        List<Ability> AbilitiesSwordsman= new List<Ability>();
        List<Ability> AbilitiesMage = new List<Ability>();
        List<Ability> AbilitiesHealer  = new List<Ability>();
        AbilitiesHealer.Add(new AbilityHealingWave());
        AbilitiesHealer.Add(new AbilityArcaneBoost());

        AbilitiesSwordsman.Add(new AbilityWhirlwindSlash());
        AbilitiesSwordsman.Add(new AbilityDoubleHit());

        AbilitiesMage.Add(new AbilityHPReaver());
        AbilitiesMage.Add(new AbilityAstralFlare());


        // PLAYERS
        
        PlayerUnit player = ((GameObject)Instantiate(PlayerPrefabs[0], new Vector3(0, 0, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player.tag = "player";
        player.AssignStats(defaultStats);
        player.AssignMap(map);
        player.AssignAbilities(AbilitiesMage);
        player.UpdateUI();
        players.Add(player);

        PlayerUnit player2 = ((GameObject)Instantiate(PlayerPrefabs[1], new Vector3(0, 2, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player2.tag = "player";
        player2.AssignStats(defaultStats);
        player2.AssignMap(map);
        player2.AssignAbilities(AbilitiesHealer);
        player2.UpdateUI();
        players.Add(player2);

        PlayerUnit player3 = ((GameObject)Instantiate(PlayerPrefabs[2], new Vector3(2, 3, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player3.tag = "player";
        player3.AssignStats(defaultStats);
        player3.AssignMap(map);
        player3.AssignAbilities(AbilitiesSwordsman);
        player3.UpdateUI();
        players.Add(player3);

        // ENEMIES

        // TODO DOESN'T ACTUALLY INITIATE A VALID ENEMY
        EnemyUnit enemy = ((GameObject)Instantiate(EnemyPrefab[0], new Vector3(-4, -3, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy.tag = "enemy";
        enemy.AssignStats(defaultStats);
        enemy.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy);

        EnemyUnit enemy2 = ((GameObject)Instantiate(EnemyPrefab[1], new Vector3(-3, -3, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy2.tag = "enemy";
        enemy2.AssignStats(defaultStats);
        enemy2.AssignMap(map);
        enemy2.UpdateUI();
        enemies.Add(enemy2);

        EnemyUnit enemy3 = ((GameObject)Instantiate(EnemyPrefab[0], new Vector3(-2, -3, 0),
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy3.tag = "enemy";
        enemy3.AssignStats(defaultStats);
        enemy3.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy3);
    }
}