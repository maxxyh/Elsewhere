using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

public class CommonInventoryManager : MonoBehaviour
{
    public List<Item> JuliusItems;
    public List<Item> KeldaItems;

    [SerializeField] UnitEquippedItemPanel unitInventoryPanel;
    [SerializeField] SelectedUnitsPanel selectedUnitsPanel;
    [SerializeField] UnitInventoryManager inventoryManager;

    List<UnitData> unitData = new List<UnitData>();
    private JObject _unitStatConfig;
    UnitData chosenUnit; 
    private string _unitStatConfigPath = Application.streamingAssetsPath + "/characterConfigEquipmentSimulated.json";

    private void OnValidate ()
    {
        Dictionary<StatString, UnitStat> JuliusStats = new Dictionary<StatString, UnitStat>();
        _unitStatConfig = JObject.Parse(File.ReadAllText(_unitStatConfigPath));
        foreach (KeyValuePair<StatString, float> pair in _unitStatConfig["Julius"]["stats"]
            .ToObject<Dictionary<StatString, float>>())
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            JuliusStats[pair.Key] = new UnitStat(pair.Value, hasLimit);
        }

        Dictionary<StatString, UnitStat> KeldaStats = new Dictionary<StatString, UnitStat>();
        _unitStatConfig = JObject.Parse(File.ReadAllText(_unitStatConfigPath));
        foreach (KeyValuePair<StatString, float> pair in _unitStatConfig["Kelda"]["stats"]
            .ToObject<Dictionary<StatString, float>>())
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            KeldaStats[pair.Key] = new UnitStat(pair.Value, hasLimit);
        }

        UnitData Julius = new UnitData("Julius", JuliusStats, JuliusItems);
        UnitData Kelda = new UnitData("Kelda", KeldaStats, KeldaItems);
        
        unitData.Add(Julius);
        unitData.Add(Kelda);

        selectedUnitsPanel.OnSlotLeftClickEvent += OnChoosingUnit;

        for (int i = 0; i < selectedUnitsPanel.selectedUnitSlots.Length; i++)
        {
            SelectedUnitSlot currSlot = selectedUnitsPanel.selectedUnitSlots[i];
            currSlot.data = unitData[i];
        }
    }

    private void Awake()
    {
        
    }

    private void ClearAllItemsInUnitInventoryPanel()
    {
        foreach(EquippedItemSlot slot in unitInventoryPanel.equippedItemSlots)
        {
            slot.Item = null;
        }
    }

    private void OnChoosingUnit(SelectedUnitSlot selectedUnit)
    {
        ClearAllItemsInUnitInventoryPanel();

        chosenUnit = selectedUnit.data;

        if (inventoryManager.unit == null)
        { 
            inventoryManager.unit = chosenUnit;
            // Debug.Log(chosenUnit.unitID);
        }
        else
        {
            inventoryManager.unit = chosenUnit;
            
        }

        Debug.Log(chosenUnit.unitID);
        inventoryManager.statPanel.SetStats(chosenUnit.stats[StatString.PHYSICAL_DAMAGE],
                                            chosenUnit.stats[StatString.MAGIC_DAMAGE],
                                            chosenUnit.stats[StatString.HIT_RATE],
                                            chosenUnit.stats[StatString.CRIT_RATE],
                                            chosenUnit.stats[StatString.ATTACK_RANGE]);
        inventoryManager.statPanel.UpdateStatValues();

        for (int i = 0; i < chosenUnit.unitItems.Count; i++)
        {
            EquippedItemSlot slot = unitInventoryPanel.equippedItemSlots[i];
            if (chosenUnit.unitItems[i] != null)
            {
                slot.Item = chosenUnit.unitItems[i];
            }
        }
    }
}
