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
    [SerializeField] UnitSelectionPanelMaxx unitSelectionPanel;
    // [SerializeField] UnitInventoryManager inventoryManager;
    [SerializeField] PreBattleUnitInventoryManager preBattleUnitInventoryManager;
    [SerializeField] UnitPersonalInventory personalInventory;
    [SerializeField] Text nameText;
    [SerializeField] ItemSaveManager itemSaveManager;

    List<UnitData> unitData = new List<UnitData>();
    UnitData chosenUnit;
    private JObject _unitStatConfig;
    private readonly string _unitStatConfigPath = Application.streamingAssetsPath + "/characterConfigEquipmentSimulated.json";

    private void Awake ()
    {
        List<string> selectedUnitIds = StaticData.SelectedUnits;
        _unitStatConfig = JObject.Parse(File.ReadAllText(_unitStatConfigPath));
        for (int i = 0; i < selectedUnitIds.Count; i++)
        {
            string unitId = selectedUnitIds[i];
            Dictionary<StatString,UnitStat> stats = ConvertStats(_unitStatConfig[unitId]["stats"].ToObject<Dictionary<StatString,string>>());
            List<Item> tempItems = i == 0 ? JuliusItems : KeldaItems;
            UnitData currUnitData = new UnitData(unitId, stats, tempItems);
            unitSelectionPanel.CreateSelectedUnitSlot(currUnitData);
        }
        
        unitSelectionPanel.OnSlotLeftClickEvent += OnChoosingUnit;
    }

    private static Dictionary<StatString, UnitStat> ConvertStats(Dictionary<StatString, string> input)
    {
        Dictionary<StatString, UnitStat> stats = new Dictionary<StatString, UnitStat>();
        foreach (KeyValuePair<StatString, string> pair in input)
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            stats[pair.Key] = new UnitStat(float.Parse(pair.Value), hasLimit);
        }

        return stats;
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
