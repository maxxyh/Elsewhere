using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using LevelSelection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public Map map;
    public HighlightMap highlightMap;
    public List<PlayerUnit> players = new List<PlayerUnit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public InitialUnitInfo initialUnitInfo;
    
    [SerializeField] UnitSaveManager unitSaveManager;
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private UnitDatabase _unitDatabase;

    [JsonConverter(typeof(StringEnumConverter))]
    //private Dictionary<string, Dictionary<StatString, string>> unitStatConfig;
    private JObject _unitStatConfig;
    private string _unitStatConfigPath = Application.streamingAssetsPath + "/characterConfigEquipmentSimulated.json";
    private JObject _classStatGrowthConfig;
    private string _classStatGrowthConfigPath = Application.streamingAssetsPath + "/classStatGrowthConfig.json";
    private JObject _characterStatGrowthConfig;
    private string _characterStatGrowthConfigPath = Application.streamingAssetsPath + "/characterStatGrowthConfig.json";
    private JObject _abilityConfig;
    private string _abilityConfigPath = Application.streamingAssetsPath + "/abilityConfig.json";
    
    public AudioClip levelMusic;

    private Action<List<PlayerUnit>> _onSaveGame;
    private Action _onWinUpdateLevel;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusicWithFade(levelMusic);
        AudioManager.Instance.SetMusicVolume(0.5f);
        map.GenerateMap();
        highlightMap.generateUIMap();
        // generatePlayers();
        GenerateUnits();
        GameAssets.MyInstance.turnScheduler.Init(players, enemies);
        
        _onWinUpdateLevel += WinUpdateLevelDatabase;
        _onSaveGame += SavePlayerData;
        GameAssets.MyInstance.turnScheduler.SetWinUpdateEvent(_onWinUpdateLevel);
        GameAssets.MyInstance.turnScheduler.SetSaveEvent(_onSaveGame);
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
    
    void GenerateUnits()
    {
        _classStatGrowthConfig = JObject.Parse(File.ReadAllText(_classStatGrowthConfigPath));
        _characterStatGrowthConfig = JObject.Parse(File.ReadAllText(_characterStatGrowthConfigPath));
        _abilityConfig = JObject.Parse(File.ReadAllText(_abilityConfigPath));

        UnitInfo[] playerInfo = initialUnitInfo.playerList;
        UnitInfo[] enemyInfo = initialUnitInfo.enemyList;
        CrystalInfo[] crystalInfo = initialUnitInfo.crystalList;

        // enemy heal-testing abilities
        List<Ability> AbilitiesHarvesterGunslinger = new List<Ability>() { new AbilityHealingWave(), new AbilityDoubleHit() };
       
        // PLAYERS
        // updating player units according to selected units 
        List<string> selectedUnitIds = StaticData.SelectedUnits;
        for (int i = 0; i < selectedUnitIds.Count; i++)
        {
            UnitDataEntry unitDataEntry =  _unitDatabase.UnitDataEntries.Find(x => x.unitName == selectedUnitIds[i]);
            playerInfo[i].UpdateUnitIdAndPrefab(unitDataEntry);
        }
        int numPlayers = selectedUnitIds.Count != 0 ? selectedUnitIds.Count : playerInfo.Length; 
        StaticData.SelectedUnits.Clear();
        
        // instantiate player units 
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerUnit player = Instantiate(playerInfo[i].UnitPrefab, playerInfo[i].UnitPositions,
                Quaternion.identity).GetComponent<PlayerUnit>();
            player.tag = "player";
            string unitId = playerInfo[i].unitID;
            UnitLoadData unitLoadData = unitSaveManager.LoadUnit(unitId);
            Dictionary<StatString, int> classStatGrowth = _classStatGrowthConfig[unitLoadData.unitClass].ToObject<Dictionary<StatString,int>>();
            Dictionary<StatString, int> characterStatGrowth = _characterStatGrowthConfig[unitId].ToObject<Dictionary<StatString,int>>();
            player.CreateUnit(unitLoadData, _abilityConfig, classStatGrowth, characterStatGrowth);
            player.AssignMap(map);
            player.UpdateUI();
            players.Add(player);
        }
        
        // ENEMIES

        for (int i = 0; i < initialUnitInfo.enemyList.Length; i++)
        {
            EnemyUnit enemy = Instantiate(enemyInfo[i].UnitPrefab, enemyInfo[i].UnitPositions, Quaternion.identity).GetComponent<EnemyUnit>();
            //enemy.gridPosition = new Vector2(0,0);
            enemy.tag = "enemy";
            string unitId = enemyInfo[i].unitID;
            UnitLoadData unitLoadData = unitSaveManager.LoadUnit(unitId, initialUnitInfo.enemyStatsFilename, true);
            Dictionary<StatString, int> classStatGrowth = _classStatGrowthConfig[unitLoadData.unitClass].ToObject<Dictionary<StatString,int>>();
            Dictionary<StatString, int> characterStatGrowth = _characterStatGrowthConfig[unitId].ToObject<Dictionary<StatString,int>>();
            enemy.CreateUnit(unitLoadData, _abilityConfig, classStatGrowth, characterStatGrowth);
            enemy.AssignMap(map);
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
    // convoy is something that we're overlooking!
    private void SavePlayerData(List<PlayerUnit> units)
    {
        Debug.Log("Saving data");
        foreach (Unit unit in units)
        {
            unitSaveManager.SaveUnit(unit);
        }
    }

    private void WinUpdateLevelDatabase()
    {
        LevelSelectManager.LevelCompleted(StaticData.LevelInformation.levelId, levelDatabase);
    }
    
}