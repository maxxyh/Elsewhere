
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// converts players inventory and equipment panel into a saveable format and calls the responding methods to save them to file
public class ItemSaveManager : MonoBehaviour
{
    [SerializeField] ItemDataBase itemDataBase;

    private const string InventoryFileName = "Inventory";
    private const string EquippedItemsFileName = "Equipped Item";

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

    private void SaveUnit(UnitData unit, string fileName)
    {
        fileName += " " + unit.unitID;
        var unitSaveData = new UnitSaveData(unit.unitID, unit.unitItems.Count);

        // probably index out of range exception
        for (int i = 0; i < unitSaveData.UnitInventory.SavedSlots.Length; i++)
        {
            Item currItem = unit.unitItems[i];
            unitSaveData.UnitInventory.SavedSlots[i] = new ItemSlotSaveData(currItem.ID, 1);
        }
        // Debug.Log(JsonSaveLoadIO.SaveUnit(unitSaveData, fileName));
        JsonSaveLoadIO.SaveUnit(unitSaveData, fileName);
    }

    public void SaveUnit(UnitData unit)
    {
        SaveUnit(unit, InventoryFileName);
    }

    public UnitData LoadUnit(string unitName)
    {
        UnitSaveData saveUnit = JsonSaveLoadIO.LoadUnit(InventoryFileName + " " + unitName);
        
        if (saveUnit == null) return null;

        List<Item> items = new List<Item>();

        for (int i = 0; i < saveUnit.UnitInventory.SavedSlots.Length; i++)
        {
            ItemSlotSaveData currItemData = saveUnit.UnitInventory.SavedSlots[i];
            if (currItemData != null)
            {
                items.Add(itemDataBase.GetItemCopy(currItemData.ItemID));
            }
        }
        UnitData loadedUnitData = new UnitData(unitName, new Dictionary<StatString, UnitStat>(), items);
        return loadedUnitData;
    }

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

    public void LoadInventory(Unit unit)
    {
        ItemContainerSaveData savedSlots = ItemSaveIO.LoadItems(InventoryFileName);

        if (savedSlots == null) return;
        
        unit.unitInventory.Clear();

        for(int i = 0; i < savedSlots.SavedSlots.Length; i++)
        {
            ItemSlot itemSlot = unit.unitInventory.ItemSlots[i];
            ItemSlotSaveData savedSlot = savedSlots.SavedSlots[i];

            if (savedSlot == null)
            {
                itemSlot.Amount = 0;
                itemSlot.Item = null;
            }
            else
            {
                itemSlot.Item = itemDataBase.GetItemCopy(savedSlot.ItemID);
                itemSlot.Amount = savedSlot.Amount;
            }
        }
        
    }

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
}
