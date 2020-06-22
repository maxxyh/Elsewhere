using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject[] EnemyPrefabs;
    public Map map;
    public HighlightMap highlightMap;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    [JsonConverter(typeof(StringEnumConverter))]
    //private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    private JObject unitStatConfig;
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

        //unitStatConfig = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Application.streamingAssetsPath + "/characterConfig.json"));
        unitStatConfig = JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + "/characterConfig.json"));

        Vector3Int[] playerPositions = levelUnitPosition.PlayerPositions;
        Vector3Int[] enemyPositions = levelUnitPosition.EnemyPositions;


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

        string[] playerNames = { "Esmeralda", "Kelda", "Julius" };
        List<Ability>[] abilitiesList = { AbilitiesMage, AbilitiesHealer, AbilitiesSwordsman };

        for (int i = 0; i < playerPositions.Count(); i++)
        {
            PlayerUnit player = ((GameObject)Instantiate(PlayerPrefabs[i], playerPositions[i],
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
            //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
            player.tag = "player";
            player.AssignStats(unitStatConfig[playerNames[i]]["stats"].ToObject<Dictionary<StatString, float>>());
            player.AssignMap(map);
            player.AssignAbilities(abilitiesList[i]);
            player.AssignIdentity((string)unitStatConfig[playerNames[i]]["name"], (string)unitStatConfig[playerNames[i]]["class"]);
            player.UpdateUI();
            players.Add(player);
        }


        // ENEMIES

        EnemyUnit enemy = ((GameObject)Instantiate(EnemyPrefabs[0], enemyPositions[0],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy.tag = "enemy";
        enemy.AssignStats(unitStatConfig["HarvesterGunslinger"]["stats"].ToObject<Dictionary<StatString, float>>());
        enemy.AssignMap(map);
        enemy.AssignIdentity((string)unitStatConfig["HarvesterGunslinger"]["name"], (string)unitStatConfig["HarvesterGunslinger"]["class"]);
        enemy.UpdateUI();
        enemies.Add(enemy);

        EnemyUnit enemy2 = ((GameObject)Instantiate(EnemyPrefabs[1], enemyPositions[1],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy2.tag = "enemy";
        enemy2.AssignStats(unitStatConfig["HarvesterTank"]["stats"].ToObject<Dictionary<StatString, float>>());
        enemy2.AssignMap(map);
        enemy2.AssignIdentity((string)unitStatConfig["HarvesterTank"]["name"], (string)unitStatConfig["HarvesterTank"]["class"]);
        enemy2.UpdateUI();
        enemies.Add(enemy2);

        EnemyUnit enemy3 = ((GameObject)Instantiate(EnemyPrefabs[0], enemyPositions[2],
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy3.tag = "enemy";
        enemy3.AssignStats(unitStatConfig["HarvesterGunslinger"]["stats"].ToObject<Dictionary<StatString, float>>());
        enemy3.AssignIdentity((string)unitStatConfig["HarvesterGunslinger"]["name"], (string)unitStatConfig["HarvesterGunslinger"]["class"]);
        enemy3.AssignMap(map);
        enemy3.UpdateUI();
        enemies.Add(enemy3);
    }
}