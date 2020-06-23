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
    public Map map;
    public HighlightMap highlightMap;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public IntialUnitInfo initialUnitInfo;
    [JsonConverter(typeof(StringEnumConverter))]
    //private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    private JObject unitStatConfig;

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

        /*Vector3Int[] playerPositions = levelUnitPosition.PlayerPositions;
        Vector3Int[] enemyPositions = levelUnitPosition.EnemyPositions;*/

        UnitInfoSO[] playerInfo = initialUnitInfo.playerList;
        UnitInfoSO[] enemyInfo = initialUnitInfo.enemyList;
        // default abilities
        List <Ability> AbilitiesSwordsman= new List<Ability>();
        List<Ability> AbilitiesMage = new List<Ability>();
        List<Ability> AbilitiesHealer  = new List<Ability>();
        AbilitiesHealer.Add(new AbilityHealingWave());
        AbilitiesHealer.Add(new AbilityArcaneBoost());

        AbilitiesSwordsman.Add(new AbilityWhirlwindSlash());
        AbilitiesSwordsman.Add(new AbilityDoubleHit());

        AbilitiesMage.Add(new AbilityHPReaver());
        AbilitiesMage.Add(new AbilityAstralFlare());

        Dictionary<string, List<Ability>> UnitAbilities = new Dictionary<string, List<Ability>>();
        UnitAbilities.Add("Kelda", AbilitiesHealer);
        UnitAbilities.Add("Julius", AbilitiesSwordsman);
        UnitAbilities.Add("Esmeralda", AbilitiesMage);

        // PLAYERS

        // string[] playerNames = { "Esmeralda", "Julius" };

        List<Ability>[] abilitiesList = { AbilitiesMage, AbilitiesSwordsman };

        for (int i = 0; i < initialUnitInfo.playerList.Length; i++)
        {
            PlayerUnit player = ((GameObject)Instantiate(playerInfo[i].UnitPrefab, playerInfo[i].UnitPositions,
            Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
            //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
            player.tag = "player";
            player.AssignStats(unitStatConfig[playerInfo[i].unitID]["stats"].ToObject<Dictionary<StatString, float>>());
            player.AssignMap(map);
            player.AssignAbilities(UnitAbilities[playerInfo[i].unitID]);
            player.AssignIdentity((string)unitStatConfig[playerInfo[i].unitID]["name"], (string)unitStatConfig[playerInfo[i].unitID]["class"]);
            player.UpdateUI();
            players.Add(player);
        }

        
        // ENEMIES

        EnemyUnit enemy = ((GameObject)Instantiate(enemyInfo[0].UnitPrefab, enemyInfo[0].UnitPositions,
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy.tag = "enemy";
        enemy.AssignStats(unitStatConfig["HarvesterGunslinger"]["stats"].ToObject<Dictionary<StatString, float>>());
        enemy.AssignMap(map);
        enemy.AssignIdentity((string)unitStatConfig["HarvesterGunslinger"]["name"], (string)unitStatConfig["HarvesterGunslinger"]["class"]);
        enemy.UpdateUI();
        enemies.Add(enemy);

        EnemyUnit enemy2 = ((GameObject)Instantiate(enemyInfo[1].UnitPrefab, enemyInfo[1].UnitPositions,
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy2.tag = "enemy";
        enemy2.AssignStats(unitStatConfig["HarvesterTank"]["stats"].ToObject<Dictionary<StatString, float>>());
        enemy2.AssignMap(map);
        enemy2.AssignIdentity((string)unitStatConfig["HarvesterTank"]["name"], (string)unitStatConfig["HarvesterTank"]["class"]);
        enemy2.UpdateUI();
        enemies.Add(enemy2);

        /*EnemyUnit enemy3 = ((GameObject)Instantiate(enemyInfo[2].UnitPrefab, enemyInfo[1].UnitPositions,
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
        //enemy.gridPosition = new Vector2(0,0);
        enemy3.tag = "enemy";
        enemy3.AssignStats(unitStatConfig["HarvesterGunslinger"]["stats"].ToObject<Dictionary<StatString, float>>());
        enemy3.AssignIdentity((string)unitStatConfig["HarvesterGunslinger"]["name"], (string)unitStatConfig["HarvesterGunslinger"]["class"]);
        enemy3.AssignMap(map);
        enemy3.UpdateUI();
        enemies.Add(enemy3);*/
    }
}