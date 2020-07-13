using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Map map;
    public HighlightMap highlightMap;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public InitialUnitInfo initialUnitInfo;

    [SerializeField] GameObject juliusGO;
    [SerializeField] GameObject keldaGO;

    [SerializeField] ItemSaveManager itemSaveManager;
    [SerializeField] private GameObject pauseMenu;

    [JsonConverter(typeof(StringEnumConverter))]
    //private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    private JObject _unitStatConfig;
    private string _unitStatConfigPath = Application.streamingAssetsPath + "/characterConfigEquipmentSimulated.json";
    private JObject _classStatGrowthConfig;
    private string _classStatGrowthConfigPath = Application.streamingAssetsPath + "/classStatGrowthConfig.json";
    private JObject _characterStatGrowthConfig;
    private string _characterStatGrowthConfigPath = Application.streamingAssetsPath + "/characterStatGrowthConfig.json";

    public AudioClip levelMusic;


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusicWithFade(levelMusic);
        AudioManager.Instance.SetMusicVolume(0.5f);
        map.GenerateMap();
        highlightMap.generateUIMap();
        // generatePlayers();
        generatePlayers2();
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

    void generatePlayers()
    {

        //unitStatConfig = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Application.streamingAssetsPath + "/characterConfig.json"));
        _unitStatConfig = JObject.Parse(File.ReadAllText(_unitStatConfigPath));
        _classStatGrowthConfig = JObject.Parse(File.ReadAllText(_classStatGrowthConfigPath));
        _characterStatGrowthConfig = JObject.Parse(File.ReadAllText(_characterStatGrowthConfigPath));

        // UnitInfo[] playerInfo = initialUnitInfo.playerList;

        UnitInfo[] enemyInfo = initialUnitInfo.enemyList;
        CrystalInfo[] crystalInfo = initialUnitInfo.crystalList;
        // default abilities
        List<Ability> AbilitiesSwordsman = new List<Ability>();
        List<Ability> AbilitiesMage = new List<Ability>();
        List<Ability> AbilitiesHealer = new List<Ability>();
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
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            UnitInfo[] playerInfo = initialUnitInfo.tutorialPlayerList;
            for (int i = 0; i < initialUnitInfo.tutorialPlayerList.Length; i++)
            {
                PlayerUnit player = (Instantiate(playerInfo[i].UnitPrefab, playerInfo[i].UnitPositions,
                Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
                //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
                player.tag = "player";
                string unitID = playerInfo[i].unitID;

                player.AssignStats(_unitStatConfig[unitID]["stats"].ToObject<Dictionary<StatString, float>>());
                player.AssignMap(map);
                player.AssignAbilities(UnitAbilities[unitID]);
                string unitClass = (string)_unitStatConfig[unitID]["class"];
                player.AssignIdentity((string)_unitStatConfig[unitID]["name"], unitClass,
                    _characterStatGrowthConfig[unitID].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass].ToObject<Dictionary<StatString, int>>());
                player.UpdateUI();
                players.Add(player);
            }
        }
        else
        {
            List<SelectableUnitTest> playerInfo = UnitSelection.selectedUnitListTest;
            Debug.Log("PLAYER INFO COUNT:" + UnitSelection.selectedUnitListTest.Count);
            for (int i = 0; i < UnitSelection.selectedUnitListTest.Count; i++)
            {
                /*                Debug.Log("PLAYER INFO COUNT:" + playerInfo.Count);
                                if (playerInfo[i] == null)
                                {
                                    Debug.Log("PLAYERINFO[i] NULL");
                                }
                                if (playerInfo[i].unitInfo.UnitPrefab == null)
                                {
                                    Debug.Log("PLAYERINFO[i] PREFABS NULL");
                                }
                                if (playerInfo[i].unitInfo.UnitPositions == null)
                                {
                                    Debug.Log("PLAYERINFO[i] POSITION NULL");
                                }*/
                PlayerUnit player = (Instantiate(playerInfo[i].unitInfo.UnitPrefab, playerInfo[i].unitInfo.UnitPositions,
                Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
                //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
                player.tag = "player";
                string unitID = playerInfo[i].unitInfo.unitID;

                player.AssignStats(_unitStatConfig[unitID]["stats"].ToObject<Dictionary<StatString, float>>());
                player.AssignMap(map);
                player.AssignAbilities(UnitAbilities[unitID]);
                string unitClass = (string)_unitStatConfig[unitID]["class"];
                player.AssignIdentity((string)_unitStatConfig[unitID]["name"], unitClass,
                    _characterStatGrowthConfig[unitID].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass].ToObject<Dictionary<StatString, int>>());
                player.UpdateUI();
                players.Add(player);
            }
        }


        // PLAYERS

        // ENEMIES

        for (int i = 0; i < initialUnitInfo.enemyList.Length; i++)
        {
            EnemyUnit enemy = Instantiate(enemyInfo[i].UnitPrefab, enemyInfo[i].UnitPositions, Quaternion.identity).GetComponent<EnemyUnit>();
            //enemy.gridPosition = new Vector2(0,0);
            enemy.tag = "enemy";
            string unitID = enemyInfo[i].unitID;

            enemy.AssignStats(_unitStatConfig[unitID]["stats"].ToObject<Dictionary<StatString, float>>());
            enemy.AssignMap(map);
            string unitClass = (string)_unitStatConfig[unitID]["class"];
            enemy.AssignIdentity((string)_unitStatConfig[unitID]["name"], unitClass,
                    _characterStatGrowthConfig[unitID].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass].ToObject<Dictionary<StatString, int>>());
            enemy.UpdateUI();
            enemies.Add(enemy);
        }

        for (int i = 0; i < initialUnitInfo.crystalList.Length; i++)
        {
            GameObject crystal = Instantiate(crystalInfo[i].UnitPrefab, crystalInfo[i].UnitPositions, Quaternion.identity);
            //enemy.gridPosition = new Vector2(0,0);
            crystal.tag = "crystal";
        }
    }

    void generatePlayers2()
    {

        //unitStatConfig = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Application.streamingAssetsPath + "/characterConfig.json"));
        _unitStatConfig = JObject.Parse(File.ReadAllText(_unitStatConfigPath));
        _classStatGrowthConfig = JObject.Parse(File.ReadAllText(_classStatGrowthConfigPath));
        _characterStatGrowthConfig = JObject.Parse(File.ReadAllText(_characterStatGrowthConfigPath));

        // UnitInfo[] playerInfo = initialUnitInfo.playerList;

        UnitInfo[] enemyInfo = initialUnitInfo.enemyList;
        CrystalInfo[] crystalInfo = initialUnitInfo.crystalList;
        // default abilities
        List<Ability> AbilitiesSwordsman = new List<Ability>();
        List<Ability> AbilitiesMage = new List<Ability>();
        List<Ability> AbilitiesHealer = new List<Ability>();
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
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            UnitInfo[] playerInfo = initialUnitInfo.tutorialPlayerList;
            for (int i = 0; i < initialUnitInfo.tutorialPlayerList.Length; i++)
            {
                PlayerUnit player = (Instantiate(playerInfo[i].UnitPrefab, playerInfo[i].UnitPositions,
                Quaternion.Euler(new Vector3()))).GetComponent<PlayerUnit>();
                //player.gridPosition = new Vector2(mapSize/2,mapSize/2);
                player.tag = "player";
                string unitID = playerInfo[i].unitID;

                player.AssignStats(_unitStatConfig[unitID]["stats"].ToObject<Dictionary<StatString, float>>());
                player.AssignMap(map);
                player.AssignAbilities(UnitAbilities[unitID]);
                string unitClass = (string)_unitStatConfig[unitID]["class"];
                player.AssignIdentity((string)_unitStatConfig[unitID]["name"], unitClass,
                    _characterStatGrowthConfig[unitID].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass].ToObject<Dictionary<StatString, int>>());
                player.UpdateUI();
                players.Add(player);
            }
        }
        else
        {

            UnitData juliusData = itemSaveManager.LoadUnit("Julius");

            PlayerUnit player = (Instantiate(juliusGO, new Vector3(-11, 1, 0), 
            Quaternion.identity)).GetComponent<PlayerUnit>();

            player.tag = "player";
            string unitID = "Julius";

            player.AssignStats(_unitStatConfig[unitID]["stats"].ToObject<Dictionary<StatString, float>>());
            // juliusData.stats = player.stats;
            player.AssignMap(map);
            player.AssignAbilities(UnitAbilities[unitID]);

            player.AssignInventory(juliusData.unitItems);

            string unitClass = (string)_unitStatConfig[unitID]["class"];
            player.AssignIdentity((string)_unitStatConfig[unitID]["name"], unitClass,
                _characterStatGrowthConfig[unitID].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass].ToObject<Dictionary<StatString, int>>());
            player.UpdateUI();
            players.Add(player);



            UnitData keldaData = itemSaveManager.LoadUnit("Kelda");

            Debug.Log(keldaData.unitItems[0]);

            PlayerUnit player1 = (Instantiate(keldaGO, new Vector3(-11, 3, 0),
            Quaternion.identity)).GetComponent<PlayerUnit>();

            player1.tag = "player";
            string unitID1 = "Kelda";

            player1.AssignStats(_unitStatConfig[unitID1]["stats"].ToObject<Dictionary<StatString, float>>());
            // juliusData.stats = player1.stats;
            player1.AssignMap(map);
            player1.AssignAbilities(UnitAbilities[unitID1]);

            player1.AssignInventory(keldaData.unitItems);

            string unitClass1 = (string)_unitStatConfig[unitID1]["class"];
            player.AssignIdentity((string)_unitStatConfig[unitID1]["name"], unitClass,
                _characterStatGrowthConfig[unitID1].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass1].ToObject<Dictionary<StatString, int>>());
            player.UpdateUI();
            players.Add(player1);
        }

        for (int i = 0; i < initialUnitInfo.enemyList.Length; i++)
        {
            EnemyUnit enemy = Instantiate(enemyInfo[i].UnitPrefab, enemyInfo[i].UnitPositions, Quaternion.identity).GetComponent<EnemyUnit>();
            //enemy.gridPosition = new Vector2(0,0);
            enemy.tag = "enemy";
            string unitID = enemyInfo[i].unitID;

            enemy.AssignStats(_unitStatConfig[unitID]["stats"].ToObject<Dictionary<StatString, float>>());
            enemy.AssignMap(map);
            string unitClass = (string)_unitStatConfig[unitID]["class"];
            enemy.AssignIdentity((string)_unitStatConfig[unitID]["name"], unitClass,
                    _characterStatGrowthConfig[unitID].ToObject<Dictionary<StatString, int>>(), _classStatGrowthConfig[unitClass].ToObject<Dictionary<StatString, int>>());
            enemy.UpdateUI();
            enemies.Add(enemy);
        }

        for (int i = 0; i < initialUnitInfo.crystalList.Length; i++)
        {
            GameObject crystal = Instantiate(crystalInfo[i].UnitPrefab, crystalInfo[i].UnitPositions, Quaternion.identity);
            //enemy.gridPosition = new Vector2(0,0);
            crystal.tag = "crystal";
        }
    }
}