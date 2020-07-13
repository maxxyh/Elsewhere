using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

[Serializable]
public class ItemSlotSaveData
{
    public string ItemID;
    public int Amount;

    public ItemSlotSaveData(string id, int amount)
    {
        ItemID = id;
        Amount = amount;
    }
}

[Serializable]
public class ItemContainerSaveData
{
    public ItemSlotSaveData[] SavedSlots;
    
    public ItemContainerSaveData(int numItems)
    {
        SavedSlots = new ItemSlotSaveData[numItems];
    }
}

[Serializable]
public class UnitSaveData
{
    public string UnitName;
    public ItemContainerSaveData UnitInventory;

    public UnitSaveData(string name, int numItems)
    {
        UnitName = name;
        UnitInventory = new ItemContainerSaveData(numItems);
    }
}