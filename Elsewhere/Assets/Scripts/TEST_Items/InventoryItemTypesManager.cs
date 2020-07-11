using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryItemTypesManager : MonoBehaviour
{
    [SerializeField] CommonInventory commonInventory;
    [SerializeField] InventoryItemTypesPanel inventoryItemTypesPanel;

    public List<Item> currItemTypeList;
    public List<Item> currInventoryItemsList = new List<Item>();

    private void Start()
    {
        //Setup Event
        inventoryItemTypesPanel.OnSlotClick += Filter;

        foreach (ItemSlot slot in commonInventory.ItemSlots)
        {
            currInventoryItemsList.Add(slot.Item);
        }
    }

    public List<Item> GetListOfItemsOfType(ItemType type)
    {
        currItemTypeList = new List<Item>();
        foreach (Item item in currInventoryItemsList)
        {
            if (item != null)
            {
                if (item.GetItemType() == type.ToString())
                {
                    currItemTypeList.Add(item);
                }
            }
        }
        return currItemTypeList;
    }

    /*public void UpdateCurrListOfItems()
    {
        
        // the unselected item might not go back to the correct tab
        for (int i = 0; i < commonInventory.ItemSlots.Count; i++)
        {
            if (commonInventory.ItemSlots[i].Item != null)
            {
                Debug.Log("Add");
                Item currItem = commonInventory.ItemSlots[i].Item;
                if (!currInventoryItemsList.Contains(currItem))
                {
                    Debug.Log("Add");
                    currInventoryItemsList.Add(currItem);
                }
            }

            if (commonInventory.ItemSlots[i].Item == null)
            {
                Debug.Log("Remove1");
                if (currInventoryItemsList.Contains(commonInventory.ItemSlots[i].Item))
                {
                    Debug.Log("Remove");
                    currInventoryItemsList.Remove(commonInventory.ItemSlots[i].Item);
                }
            }
        }
    }*/

    public void ClearCurrentInventory()
    {
        foreach(ItemSlot slot in commonInventory.ItemSlots)
        {
            slot.Item = null;
        }
    }

    public void Filter(InventoryItemTypeSlot slot)
    {
        
        currItemTypeList = GetListOfItemsOfType(slot.GetSlotType());
        
        ClearCurrentInventory();

        for (int i = 0; i < commonInventory.ItemSlots.Count; i++)
        {
            if (i < currItemTypeList.Count)
            {
                if (currItemTypeList[i] != null)
                {
                    commonInventory.ItemSlots[i].Item = currItemTypeList[i];
                }
            }
            else
            {
                return;
            }
        }
    }
}
