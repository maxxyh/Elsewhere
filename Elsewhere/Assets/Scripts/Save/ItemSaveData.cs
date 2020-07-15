using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlotSaveData
{
    public string itemId;
    public int amount;

    public ItemSlotSaveData(string id, int amount)
    {
        itemId = id;
        this.amount = amount;
    }
}

[Serializable]
public class ItemContainerSaveData
{
    public ItemSlotSaveData[] savedSlots;
    
    public ItemContainerSaveData(int numItems)
    {
        savedSlots = new ItemSlotSaveData[numItems];
    }

    public void AddItems(List<ItemSlotData> occupiedItemSlots)
    {
        for (int i = 0; i< occupiedItemSlots.Count; i++)
        {
            Item currItem = occupiedItemSlots[i].Item;
            savedSlots[i] = new ItemSlotSaveData(currItem.ID, occupiedItemSlots[i].Amount);
        }
    }
    public void AddItems(List<Item> items)
    {
        for (int i = 0; i< items.Count; i++)
        {
            Item currItem = items[i];
            savedSlots[i] = new ItemSlotSaveData(currItem.ID, 1);
        }
    }

    public List<Item> GetCopyOfItems(ItemDataBase itemDataBase)
    {
        List<Item> items = new List<Item>();
        foreach (ItemSlotSaveData itemSlotSaveData in savedSlots)
        {
            for (int i = 0; i < itemSlotSaveData.amount; i++)
            {
                Item item = itemDataBase.GetItemCopy(itemSlotSaveData.itemId);
                items.Add(item);
            }
        }

        return items;
    }
}