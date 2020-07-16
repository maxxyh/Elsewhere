using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    private List<SelectedUnitSlot> _selectedUnitSlots = new List<SelectedUnitSlot>();
    public int limit;
    public Text limitText;
    public Text noOfUnits;
    
    private JObject _unlockedCharacters;
    private readonly string _unlockedCharactersPath = Application.streamingAssetsPath + "/unlockedCharactersConfig.json";
    private JObject _abilityConfig;
    private string _abilityConfigPath = Application.streamingAssetsPath + "/abilityConfig.json";

    
    [SerializeField] MajorStatPanel majorStatPanel;
    [SerializeField] private UnitSelectionPanelMaxx unitSelectionPanel;
    [SerializeField] private UnitSaveManager unitSaveManager;

    private void Awake()
    {
        _unlockedCharacters = JObject.Parse(File.ReadAllText(_unlockedCharactersPath));
        _abilityConfig = JObject.Parse(File.ReadAllText(_abilityConfigPath));

        string[] unlockedCharacterIds = _unlockedCharacters["unlocked"].ToObject<string[]>();
        limit = StaticData.LevelInformation.GetNumPlayers();
        limitText.text = "             /" + limit + " units"; 
        unitSelectionPanel.OnSlotLeftClickEvent += OnToggleSelectUnit;
        unitSelectionPanel.OnSlotMouseEnterEvent += OnToggleUnitStats;

        for (int i = 0; i < unlockedCharacterIds.Length; i++)
        {
            string unitId = unlockedCharacterIds[i];
            UnitData unitData = unitSaveManager.LoadUnitData(unitId);
            unitSelectionPanel.CreateUnitSelectionSlot(unitData);
        }
    }

    private static Dictionary<StatString, UnitStat> ConvertStats(Dictionary<StatString, string> input)
    {
        Dictionary<StatString,UnitStat> stats = new Dictionary<StatString, UnitStat>();
        foreach (KeyValuePair<StatString, string> pair in input)
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            stats[pair.Key] = new UnitStat(float.Parse(pair.Value), hasLimit);
        }

        return stats;
    }

    private void Update()
    {
        noOfUnits.text = _selectedUnitSlots.Count.ToString();
    }


    public void OnStartGameButton()
    {
        if (_selectedUnitSlots.Count < limit)
        {
            Debug.LogError("Cannot start because not enough units");
        }
        else if (this._selectedUnitSlots.Count == limit)
        {
            StaticData.SelectedUnits = _selectedUnitSlots.Select(x => x.UnitName).ToList();
            //SceneManager.LoadScene(nextSceneBuildIndex);
            SceneManager.LoadScene("InventoryManagement");
        }
    }
    
    private void OnToggleSelectUnit(SelectedUnitSlot selectedUnit)
    {
        // toggle style 
        if (!_selectedUnitSlots.Contains(selectedUnit))
        {
            if (_selectedUnitSlots.Count < limit)
            {
                _selectedUnitSlots.Add(selectedUnit); 
                ((UnitSelectionSlot)selectedUnit).SetGrayscale(true);
            }
            else
            {
                Debug.LogError("Trying to select more than limit");
            }
        }
        else
        {
            _selectedUnitSlots.Remove(selectedUnit);
            ((UnitSelectionSlot)selectedUnit).SetGrayscale(false);
        }
    }

    private void OnToggleUnitStats(SelectedUnitSlot selectedUnit)
    {
        majorStatPanel.UpdateStatsUI(selectedUnit.data.stats);
        majorStatPanel.ClearAllAbilitiesFromPanel();
        foreach (string abilityName in selectedUnit.data.unitAbilities)
        {
            majorStatPanel.AddAbilityToPanel((string) _abilityConfig[abilityName]["name"], 
                (string) _abilityConfig[abilityName]["description"], StaticData.AbilityReference[abilityName].GetManaCost());
        }
    }
}
