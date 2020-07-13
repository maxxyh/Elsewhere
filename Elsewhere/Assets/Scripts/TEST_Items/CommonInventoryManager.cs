using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using UnityEngine.UI;

public class CommonInventoryManager : MonoBehaviour
{
    public List<Item> JuliusItems;
    public List<Item> KeldaItems;

    // [SerializeField] UnitEquippedItemPanel unitInventoryPanel;
    [SerializeField] SelectedUnitsPanel selectedUnitsPanel;
    // [SerializeField] UnitInventoryManager inventoryManager;
    [SerializeField] PreBattleUnitInventoryManager preBattleUnitInventoryManager;
    [SerializeField] UnitPersonalInventory personalInventory;
    [SerializeField] Text nameText;
    [SerializeField] ItemSaveManager itemSaveManager;

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
    private void OnDestroy()
    {
        foreach (UnitData data in unitData)
        {
            itemSaveManager.SaveUnit(data);
        }
    }

    private void Update()
    {
        if (chosenUnit != null)
        {
            nameText.text = chosenUnit.unitID;
        }
        else
        {
            nameText.text = "";
        }
    }

    private void ClearAllItemsInUnitInventoryPanel()
    {
        foreach(EquippedItemSlot slot in personalInventory.ItemSlots)
        {
            slot.Item = null;
        }
    }

    private void OnChoosingUnit(SelectedUnitSlot selectedUnit)
    {
        ClearAllItemsInUnitInventoryPanel();

        chosenUnit = selectedUnit.data;

        if (preBattleUnitInventoryManager.unit == null)
        {
            preBattleUnitInventoryManager.unit = chosenUnit;
            // Debug.Log(chosenUnit.unitID);
        }
        else
        {
            preBattleUnitInventoryManager.unit = chosenUnit;
            
        }

        /*preBattleUnitInventoryManager.statPanel.SetStats(chosenUnit.stats[StatString.PHYSICAL_DAMAGE],
                                            chosenUnit.stats[StatString.MAGIC_DAMAGE],
                                            chosenUnit.stats[StatString.HIT_RATE],
                                            chosenUnit.stats[StatString.CRIT_RATE],
                                            chosenUnit.stats[StatString.ATTACK_RANGE]);
        preBattleUnitInventoryManager.statPanel.UpdateStatValues();*/

        for (int i = 0; i < chosenUnit.unitItems.Count; i++)
        {
            ItemSlot slot = personalInventory.ItemSlots[i];
            if (chosenUnit.unitItems[i] != null)
            {
                slot.Item = chosenUnit.unitItems[i];
            }
        }
    }
}
