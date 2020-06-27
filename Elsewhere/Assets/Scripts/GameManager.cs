using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    public Map map;
    public HighlightMap highlightMap;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public InitialUnitInfo initialUnitInfo;
    [SerializeField] private GameObject pauseMenu;

   [JsonConverter(typeof(StringEnumConverter))]
    //private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    private JObject unitStatConfig;

    public AudioClip levelMusic;

 
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusicWithFade(levelMusic);
        AudioManager.Instance.SetMusicVolume(0.5f);
        map.GenerateMap();
        highlightMap.generateUIMap();
        generatePlayers();
        GameAssets.MyInstance.turnScheduler.Init(players, enemies);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }
    }

    public void OnResumeButton()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

void generatePlayers() {

        //unitStatConfig = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Application.streamingAssetsPath + "/characterConfig.json"));
        unitStatConfig = JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + "/characterConfigEquipmentSimulated.json"));

        UnitInfo[] playerInfo = initialUnitInfo.playerList;
        UnitInfo[] enemyInfo = initialUnitInfo.enemyList;
        CrystalInfo[] crystalInfo = initialUnitInfo.crystalList;

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

        // enemy heal-testing abilities
        List<Ability> AbilitiesHarvesterGunslinger = new List<Ability>() { new AbilityHealingWave(), new AbilityDoubleHit() };

        // PLAYERS

        for (int i = 0; i < initialUnitInfo.playerList.Length; i++)
        {
            PlayerUnit player = (Instantiate(playerInfo[i].UnitPrefab, playerInfo[i].UnitPositions,
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

        for (int i = 0; i < initialUnitInfo.enemyList.Length; i++)
        {
            EnemyUnit enemy = (Instantiate(enemyInfo[i].UnitPrefab, enemyInfo[i].UnitPositions,
            Quaternion.Euler(new Vector3()))).GetComponent<EnemyUnit>();
            //enemy.gridPosition = new Vector2(0,0);
            enemy.tag = "enemy";
            enemy.AssignStats(unitStatConfig[enemyInfo[i].unitID]["stats"].ToObject<Dictionary<StatString, float>>());
            enemy.AssignMap(map);
            enemy.AssignIdentity((string)unitStatConfig[enemyInfo[i].unitID]["name"], (string)unitStatConfig["HarvesterGunslinger"]["class"]);
            enemy.UpdateUI();
            enemies.Add(enemy);
        }

        for (int i = 0; i < initialUnitInfo.crystalList.Length; i++)
        {
            GameObject crystal = (Instantiate(crystalInfo[i].UnitPrefab, crystalInfo[i].UnitPositions,
            Quaternion.Euler(new Vector3())));
            //enemy.gridPosition = new Vector2(0,0);
            crystal.tag = "crystal";
        }
    }
}