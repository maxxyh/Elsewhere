using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject[] EnemyPrefabs;
    public Map map;
    public HighlightMap highlightMap;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    [JsonConverter(typeof(StringEnumConverter))]
    private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    public LevelUnitPosition levelUnitPosition;

    public Camera worldCamera;

  

    // Start is called before the first frame update
    void Start()
    {
        map.GenerateMap();
        highlightMap.generateUIMap();
        generatePlayers();
        GameAssets.MyInstance.turnScheduler.Init(players, enemies);
    }

    void generatePlayers() {

        unitStatConfig = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<StatString, string>>>(File.ReadAllText(@"Assets\Scripts\characterConfig.json"));

        Vector3Int[] playerPositions = levelUnitPosition.PlayerPositions;
        Vector3Int[] enemyPositions = levelUnitPosition.EnemyPositions;

        //dynamic temp = JObject.Parse(File.ReadAllText(@"Assets\Scripts\characterConfig.json"));
        //Debug.Log(temp.Julius.HP);

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
        player.AssignStats(unitStatConfig["Esmeralda"]);
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
        player3.AssignStats(unitStatConfig["Julius"]);
        player3.AssignMap(map);
        player3.AssignAbilities(AbilitiesSwordsman);
        player3.UpdateUI();
        players.Add(player3);

        // ENEMIES

        EnemyUnit enemy = ((GameObject)Instantiate(EnemyPrefabs[0], enemyPositions[0],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy.tag = "enemy";
        enemy.AssignStats(unitStatConfig["HarvesterGunslinger"]);
        enemy.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy);

        EnemyUnit enemy2 = ((GameObject)Instantiate(EnemyPrefabs[1], enemyPositions[1],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy2.tag = "enemy";
        enemy2.AssignStats(unitStatConfig["HarvesterSwordsman"]);
        enemy2.AssignMap(map);
        enemy2.UpdateUI();
        enemies.Add(enemy2);

        EnemyUnit enemy3 = ((GameObject)Instantiate(EnemyPrefabs[0], enemyPositions[2],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy3.tag = "enemy";
        enemy3.AssignStats(unitStatConfig["HarvesterGunslinger"]);
        enemy3.AssignMap(map);
        enemy.UpdateUI();
        enemies.Add(enemy3);
    }
}