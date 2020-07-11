
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
    private void SaveItems(IList<ItemSlot> itemSlots, string fileName)
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
        ItemSaveIO.SaveItems(saveData, fileName);
    }

    public void SaveInventory(InBattleUnitInventoryManager unit)
    {
        SaveItems(unit.inventory.ItemSlots, InventoryFileName);
    }

    public void SaveEquippedItems(InBattleUnitInventoryManager unit)
    {
        SaveItems(unit.equippedItemsPanel.equippedItemSlots, EquippedItemsFileName);
    }

    public void LoadInventory(InBattleUnitInventoryManager unit)
    {
        ItemContainerSaveData savedSlots = ItemSaveIO.LoadItems(InventoryFileName);

        if (savedSlots == null) return;
        
        unit.inventory.Clear();

        for(int i = 0; i < savedSlots.SavedSlots.Length; i++)
        {
            ItemSlot itemSlot = unit.inventory.ItemSlots[i];
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

    public void LoadEquippedItems(InBattleUnitInventoryManager unit)
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
            unit.inventory.AddItem(item);
            // Debug.Log(unit.inventory.ItemSlots.Count);
            /*Debug.Log(unit.inventory.ItemSlots[0].Item);
            Debug.Log(unit.inventory.ItemSlots[0].Amount);
            Debug.Log(unit.inventory.ItemSlots[2].icon.color.a);*/
            if (item is EquippableItem)
            {
                unit.Equip((EquippableItem) item);
            }
            else
            {
                Debug.Log("Not equippable");
            }
        }
    }

    
}
