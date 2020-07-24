
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// converts players inventory and equipment panel into a savable format and calls the responding methods to save them to file
public class UnitSaveManager : MonoBehaviour
{
    [SerializeField] ItemDataBase itemDataBase;

    private Dictionary<string, UnitSaveData> _saveDatabase;
    
    private const string UnitsSaveFileName = "unitsSaveData";

    private const string CommonInventorySaveFileName = "commonInventorySaveData";
    private void SaveUnit(UnitData unit, string fileName)
    {
        // load the old database
        _saveDatabase = JsonSaveLoadIO.LoadAllUnits(UnitsSaveFileName);
        
        // get old unitSaveData
        UnitSaveData oldUnitSaveData = _saveDatabase[unit.unitID];
        
        // create new unitSaveData from old unitSaveData
        var unitSaveData = new UnitSaveData(oldUnitSaveData, unit.unitItems, ConvertStatsToBaseInt(unit.stats));

        // update old database with new unitSaveData
        _saveDatabase[unit.unitID] = unitSaveData;
        
        // write out to file
        // Debug.Log(JsonSaveLoadIO.SaveUnit(unitSaveData, fileName));
        JsonSaveLoadIO.SaveAllUnits(_saveDatabase, fileName);
    }

    private void SaveUnit(Unit unit, string fileName)
    {
        // load the old database
        _saveDatabase = JsonSaveLoadIO.LoadAllUnits(UnitsSaveFileName);
        
        // create new unitSaveData
        List<ItemSlotData> occupiedItemSlots = unit.unitInventory.Where(x => x.Item != null).ToList();
        IEnumerable<string> unitAbilities = unit.abilities.Select(x =>
        {
            foreach(KeyValuePair<string,Ability> pair in StaticData.AbilityReference)
            {
                if (pair.Value.Equals(x))
                {
                    return pair.Key;
                }
            }
            return null;
        });
        var unitSaveData = new UnitSaveData(unit.characterName, unit.characterClass, unit.level,
            occupiedItemSlots, ConvertStatsToBaseInt(unit.stats), unitAbilities.ToList());

        // update old database with new unitSaveData
        _saveDatabase[unit.characterName] = unitSaveData;
        
        // write out to file
        // Debug.Log(JsonSaveLoadIO.SaveUnit(unitSaveData, fileName));
        JsonSaveLoadIO.SaveAllUnits(_saveDatabase, fileName);
    }

    public void SaveUnit(UnitData unit)
    {
        SaveUnit(unit, UnitsSaveFileName);
    }

    public void SaveUnit(Unit unit)
    {
        SaveUnit(unit, UnitsSaveFileName);
    }

    public UnitLoadData LoadUnit(string unitId)
    {
        return LoadUnit(unitId, UnitsSaveFileName);
    }
    
    public UnitLoadData LoadUnit(string unitId, string filename, bool enemy = false)
    {
        // minor difference for loading enemy units
        _saveDatabase = enemy ? JsonSaveLoadIO.LoadAllEnemyUnits(filename) : JsonSaveLoadIO.LoadAllUnits(filename);    
        
        UnitSaveData saveUnit = _saveDatabase[unitId];

        if (saveUnit == null)
        {
            Debug.LogError("Unit not found in database. Updating with firstSave info.");
            _saveDatabase = JsonSaveLoadIO.LoadAllUnits("firstSave");
            saveUnit = _saveDatabase[unitId];
        }
        
        return new UnitLoadData(saveUnit, itemDataBase);
    }

    public UnitData LoadUnitData(string unitId)
    {
        _saveDatabase = JsonSaveLoadIO.LoadAllUnits(UnitsSaveFileName);
        UnitSaveData saveUnit = _saveDatabase[unitId];

        if (saveUnit == null)
        {
            Debug.LogError("Unit not found in database. Updating with firstSave info.");
            _saveDatabase = JsonSaveLoadIO.LoadAllUnits("firstSave");
            saveUnit = _saveDatabase[unitId];
        }

        List<Item> items = saveUnit.unitInventory.GetCopyOfItems(itemDataBase);

        UnitData loadedUnitData = new UnitData(saveUnit.unitName, ConvertStatsToUnitStat(saveUnit.unitStats), items,
            saveUnit.unitAbilities, saveUnit.unitLevel, saveUnit.unitExp);
        return loadedUnitData;
    }
    
    private void SaveItems(IList<ItemSlot> itemSlots, string fileName)
    {
        var saveData = new ItemContainerSaveData(itemSlots.Count);

        for (int i = 0; i < saveData.savedSlots.Length; i++)
        {
            ItemSlot currSlot = itemSlots[i];

            if (currSlot.Item == null)
            {
                saveData.savedSlots[i] = null;
            }
            else
            {
                saveData.savedSlots[i] = new ItemSlotSaveData(currSlot.Item.ID, currSlot.Amount);
            }
        }
        JsonSaveLoadIO.SaveItems(saveData, fileName);
    }

    public void SaveInventory(IList<ItemSlot> items)
    {
        SaveItems(items, CommonInventorySaveFileName);
    }
    
    // DANGEROUS BUT EASY DEFAULT TO AMOUNT = 1
    private void SaveItems(IList<Item> items, string fileName)
    {
        var saveData = new ItemContainerSaveData(items.Count);

        for (int i = 0; i < saveData.savedSlots.Length; i++)
        {
            if (items[i] == null)
            {
                saveData.savedSlots[i] = null;
            }
            else
            {
                saveData.savedSlots[i] = new ItemSlotSaveData(items[i].ID, 1);
            }
        }
        JsonSaveLoadIO.SaveItems(saveData, fileName);
    }
    
    public void SaveInventory(IList<Item> items)
    {
        SaveItems(items, CommonInventorySaveFileName);
    }
    
    public List<Item> LoadInventory()
    {
        ItemContainerSaveData itemContainer = JsonSaveLoadIO.LoadItems(CommonInventorySaveFileName);

        List<Item> items = new List<Item>();
        
        foreach (ItemSlotSaveData itemSlotSave in itemContainer.savedSlots)
        {
            if (itemSlotSave != null)
            {
                items.Add(itemDataBase.GetItemCopy(itemSlotSave.itemId));
            }
        }
        return items;
    }
    
    
    // removal of stat bonuses is done here.
    private static Dictionary<StatString, int> ConvertStatsToBaseInt(Dictionary<StatString, UnitStat> stats)
    {
        Dictionary<StatString, int> output = new Dictionary<StatString, int>();
        foreach (KeyValuePair<StatString, UnitStat> pair in stats)
        {
            output.Add(pair.Key, (int) pair.Value.baseValue);
        }

        return output;
    }
    
    private static Dictionary<StatString, UnitStat> ConvertStatsToUnitStat(Dictionary<StatString, int> stats)
    {
        Dictionary<StatString, UnitStat> output = new Dictionary<StatString, UnitStat>();
        foreach (KeyValuePair<StatString, int> pair in stats)
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            output.Add(pair.Key, new UnitStat(pair.Value, hasLimit));
        }
        return output;
    }


    #region deprecated
    
    
    /*public void LoadInventory(Unit unit)
    {
        ItemContainerSaveData savedSlots = ItemSaveIO.LoadItems(UnitsSaveFileName);

        if (savedSlots == null) return;
        
        unit.unitInventory.Clear();

        for(int i = 0; i < savedSlots.savedSlots.Length; i++)
        {
            ItemSlot itemSlot = unit.unitInventory[i];
            ItemSlotSaveData savedSlot = savedSlots.savedSlots[i];

            if (savedSlot == null)
            {
                itemSlot.Amount = 0;
                itemSlot.Item = null;
            }
            else
            {
                itemSlot.Item = itemDataBase.GetItemCopy(savedSlot.itemId);
                itemSlot.Amount = savedSlot.amount;
            }
        }
        
    }*/
    
    // can pass an array or list
    /*private void SaveItems(IList<ItemSlot> itemSlots, string fileName)
    {
        var saveData = new ItemContainerSaveData(itemSlots.Count);

        for (int i = 0; i < saveData.SavedSlots.Length; i++)
        {
            ItemSlot currSlot = itemSlots[i];

            if (currSlot.Item == null)
            {
                saveData.SavedSlots[i] = null;
            }
            else
            {
                saveData.SavedSlots[i] = new ItemSlotSaveData(currSlot.Item.ID, currSlot.Amount);
            }
        }
        JsonSaveLoadIO.SaveItems(saveData, fileName);
    }

    public void SaveInventory(Unit unit)
    {
        SaveItems(unit.unitInventory.ItemSlots, InventoryFileName);
    }*/

    /*public void SaveItemsList(List<Item> items, string fileName)
{
    *//*var saveData = new ItemContainerSaveData(items.Count);

        for (int i = 0; i < saveData.SavedSlots.Length; i++)
        {
            ItemSlot currSlot = items[i];

            if (currSlot.Item == null)
            {
                saveData.SavedSlots[i] = null;
            }
            else
            {
                saveData.SavedSlots[i] = new ItemSlotSaveData(currSlot.Item.ID, currSlot.Amount);
            }
        }
        JsonSaveLoadIO.SaveItems(saveData, fileName);*//*
    }*/

    /*public void SaveEquippedItems(Unit unit)
    {
        SaveItems(unit.unitInventory.equippedItemSlots, EquippedItemsFileName);
    }*/


    /*public void SaveUnitData(Unit unit, string fileName)
    {
        // SaveItems(unit.unitInventory.ItemSlots, unit.unitID + " " + InventoryFileName);
        List<Item> unitItems = new List<Item>();
        foreach (ItemSlot slot in unit.unitInventory.ItemSlots)
        {
            if (slot.Item != null)
            {
                unitItems.Add(slot.Item);
            }
        }
        fileName = fileName + " " + unit.characterName;
        UnitData unitData = new UnitData(unit.characterName, unit.stats, unitItems);
        ItemSaveIO.SaveUnit(unitData, fileName);
    }*/
    /*public void SaveUnitData(UnitData data, string fileName)
    {
        fileName += " " + data.unitID;
        ItemSaveIO.SaveUnit(data, fileName);
    }

    public void SaveUnit(UnitData data)
    {
        SaveUnitData(data, "Unit Data");
    }

    public UnitData LoadUnitData(string unitName)
    {
        return ItemSaveIO.LoadUnit("Unit Data" + " " + unitName);
    }*/

    /*public void LoadEquippedItems(InBattleUnitInventoryManager unit)
    {
        ItemContainerSaveData savedSlots = ItemSaveIO.LoadItems(EquippedItemsFileName);
        
        if (savedSlots == null)
        {
            return;
        }
        
        foreach (ItemSlotSaveData savedSlot in savedSlots.SavedSlots)
        {
            if (savedSlot == null)
            {
                continue;
            }
            Item item = itemDataBase.GetItemCopy(savedSlot.ItemID);
            unit.unitPersonalInventory.AddItem(item);
            // Debug.Log(unit.inventory.ItemSlots.Count);
            *//*Debug.Log(unit.inventory.ItemSlots[0].Item);
            Debug.Log(unit.inventory.ItemSlots[0].Amount);
            Debug.Log(unit.inventory.ItemSlots[2].icon.color.a);*//*
            if (item is EquippableItem)
            {
                unit.Equip((EquippableItem) item);
            }
            else
            {
                Debug.Log("Not equippable");
            }
        }
    }*/
    
    
    #endregion

}
