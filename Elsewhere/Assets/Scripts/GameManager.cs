using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;

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
    private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    public LevelUnitPosition levelUnitPosition;

    public Camera worldCamera;

    [JsonConverter(typeof(StringEnumConverter))]
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

        unitStatConfig = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<StatString, string>>>(File.ReadAllText(@"Assets\Scripts\characterConfig.json"));

        Vector3Int[] playerPositions = levelUnitPosition.PlayerPositions;
        Vector3Int[] enemyPositions = levelUnitPosition.EnemyPositions;


        // default stats 
        /*
        defaultStats.Add(StatString.PHYSICAL_DAMAGE, 6);
        defaultStats.Add(StatString.MAGIC_DAMAGE, 5);
        defaultStats.Add(StatString.MANA, 20);
        defaultStats.Add(StatString.HP, 10);
        defaultStats.Add(StatString.ARMOR, 3);
        defaultStats.Add(StatString.MAGIC_RES, 2);
        defaultStats.Add(StatString.MOVEMENT_RANGE, 4);
        defaultStats.Add(StatString.ATTACK_RANGE, 2);
        */

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
        
        PlayerUnit player = ((GameObject)Instantiate(PlayerPrefabs[0], playerPositions[0],
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player.tag = "player";
        player.AssignStats(unitStatConfig["Julius"]);
        player.AssignMap(map);
        player.AssignAbilities(AbilitiesMage);
        player.UpdateUI();
        players.Add(player);

        PlayerUnit player2 = ((GameObject)Instantiate(PlayerPrefabs[1], playerPositions[1],
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player2.tag = "player";
        player2.AssignStats(unitStatConfig["Kelda"]);
        player2.AssignMap(map);
        player2.AssignAbilities(AbilitiesHealer);
        player2.UpdateUI();
        players.Add(player2);

        PlayerUnit player3 = ((GameObject)Instantiate(PlayerPrefabs[2], playerPositions[2],
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
        //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
        player3.tag = "player";
        player3.AssignStats(unitStatConfig["Esmeralda"]);
        player3.AssignMap(map);
        player3.AssignAbilities(AbilitiesSwordsman);
        player3.UpdateUI();
        players.Add(player3);

        // ENEMIES

        // TODO DOESN'T ACTUALLY INITIATE A VALID ENEMY
        EnemyUnit enemy = ((GameObject)Instantiate(EnemyPrefab[0], enemyPositions[0],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy.tag = "enemy";
        enemy.AssignStats(unitStatConfig["HarvesterGunslinger"]);
        enemy.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy);

        EnemyUnit enemy2 = ((GameObject)Instantiate(EnemyPrefab[1], enemyPositions[1],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy2.tag = "enemy";
        enemy2.AssignStats(unitStatConfig["HarvesterSwordsman"]);
        enemy2.AssignMap(map);
        enemy2.UpdateUI();
        enemies.Add(enemy2);

        EnemyUnit enemy3 = ((GameObject)Instantiate(EnemyPrefab[0], enemyPositions[2],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy3.tag = "enemy";
        enemy3.AssignStats(unitStatConfig["HarvesterGunslinger"]);
        enemy3.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy3);
    }
}