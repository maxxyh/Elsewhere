using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonInventoryManager : MonoBehaviour
{
    // [SerializeField] UnitEquippedItemPanel unitInventoryPanel;
    [SerializeField] UnitSelectionPanelMaxx unitSelectionPanel;
    // [SerializeField] UnitInventoryManager inventoryManager;
    [SerializeField] PreBattleUnitInventoryManager preBattleUnitInventoryManager;
    [SerializeField] UnitPersonalInventory personalInventory;
    [SerializeField] Text nameText;
    [SerializeField] UnitSaveManager unitSaveManager;
    [SerializeField] private ChangeScene changeSceneManger;

    private List<UnitData> unitDataList = new List<UnitData>();
    private UnitData _chosenUnitData;

    private void Awake ()
    {
        List<string> selectedUnitIds = StaticData.SelectedUnits;
        for (int i = 0; i < selectedUnitIds.Count; i++)
        {
            string unitId = selectedUnitIds[i];
            UnitData unitData = unitSaveManager.LoadUnitData(unitId);
            unitSelectionPanel.CreateSelectedUnitSlot(unitData);
            unitDataList.Add(unitData);
        }

        unitSelectionPanel.OnSlotLeftClickEvent += OnChoosingUnit;
        
        changeSceneManger.gameObject.SetActive(true);
    }
    
    private void OnDestroy()
    {
        foreach (UnitData data in unitDataList)
        {
            unitSaveManager.SaveUnit(data);
        }
    }

    private void Update()
    {
        if (_chosenUnitData != null)
        {
            nameText.text = _chosenUnitData.unitID;
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

        _chosenUnitData = selectedUnit.data;

        if (preBattleUnitInventoryManager.unit == null)
        {
            preBattleUnitInventoryManager.unit = _chosenUnitData;
            // Debug.Log(chosenUnit.unitID);
        }
        else
        {
            preBattleUnitInventoryManager.unit = _chosenUnitData;
            
        }

        /*preBattleUnitInventoryManager.statPanel.SetStats(chosenUnit.stats[StatString.PHYSICAL_DAMAGE],
                                            chosenUnit.stats[StatString.MAGIC_DAMAGE],
                                            chosenUnit.stats[StatString.HIT_RATE],
                                            chosenUnit.stats[StatString.CRIT_RATE],
                                            chosenUnit.stats[StatString.ATTACK_RANGE]);
        preBattleUnitInventoryManager.statPanel.UpdateStatValues();*/

        for (int i = 0; i < _chosenUnitData.unitItems.Count; i++)
        {
            ItemSlot slot = personalInventory.ItemSlots[i];
            if (_chosenUnitData.unitItems[i] != null)
            {
                slot.Item = _chosenUnitData.unitItems[i];
            }
        }
    }

    public void OnStartLevelButton()
    {
        changeSceneManger.OnlickChangeSceneButton(StaticData.LevelInformation.sceneName);
    }
}
