using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class UnitSaveData
{
    public string unitName;
    public string unitClass;
    public int unitLevel;
    public int unitExp;
    public List<string> unitAbilities;
    public Dictionary<StatString,int> unitStats;
    public ItemContainerSaveData unitInventory;
    
    [JsonConstructor]
    public UnitSaveData(string name, string classInput, int numItems, int unitLevelInput, int unitExpInput, Dictionary<StatString,int> stats, List<string> abilities)
    {
        unitName = name;
        unitClass = classInput;
        unitLevel = unitLevelInput;
        unitExp = unitExpInput;
        unitInventory = new ItemContainerSaveData(numItems);
        unitStats = stats;
        unitAbilities = abilities;
    }
    
    public UnitSaveData(string name, string classInput, Level unitLevel, List<ItemSlotData> itemSlots, Dictionary<StatString,int> stats, List<string> abilities)
    {
        unitName = name;
        unitClass = classInput;
        this.unitLevel = unitLevel.currentLevel;
        unitExp = unitLevel.currentExperience;
        unitInventory = new ItemContainerSaveData(itemSlots.Count);
        unitInventory.AddItems(itemSlots);
        unitStats = stats;
        unitAbilities = abilities;
    }
    public UnitSaveData(UnitSaveData oldUnitSaveData, List<Item> items, Dictionary<StatString,int> stats)
    {
        unitName = oldUnitSaveData.unitName;
        unitClass = oldUnitSaveData.unitClass;
        unitLevel = oldUnitSaveData.unitLevel;
        unitExp = oldUnitSaveData.unitExp;
        unitInventory = new ItemContainerSaveData(items.Count);
        unitInventory.AddItems(items);
        unitStats = stats;
        unitAbilities = oldUnitSaveData.unitAbilities;
    }
}

[Serializable]
public class UnitLoadData
{
    public string unitName;
    public string unitClass;
    public int unitLevel;
    public int unitExp;
    public List<string> unitAbilities;
    public Dictionary<StatString,int> unitStats;
    public List<Item> unitInventory;

    public UnitLoadData(UnitSaveData unitSaveData, ItemDataBase itemDataBase)
    {
        unitName = unitSaveData.unitName;
        unitClass = unitSaveData.unitClass;
        unitLevel = unitSaveData.unitLevel;
        unitExp = unitSaveData.unitExp;
        unitAbilities = unitSaveData.unitAbilities;
        unitStats = unitSaveData.unitStats;
        unitInventory = unitSaveData.unitInventory.GetCopyOfItems(itemDataBase);
    }
}