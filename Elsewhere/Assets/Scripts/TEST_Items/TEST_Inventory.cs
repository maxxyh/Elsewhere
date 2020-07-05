using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TEST_Inventory : MonoBehaviour, ITEST_Inventory
{
    [FormerlySerializedAs("items")]
    [SerializeField] List<TEST_Item> startingItems;
    [SerializeField] Transform itemsParent;
    [SerializeField] TEST_ItemSlot[] itemSlots;

    public event Action<TEST_Item> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
        SetStartingItems();
    }
    private void OnValidate()
    {
        if (itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<TEST_ItemSlot>();
        }
    }

    private void SetStartingItems()
    {
        int i = 0;
        for (; i < startingItems.Count && i < itemSlots.Length; i++)
        {
            // itemSlots[i].Item = startingItems[i].GetCopy() to support stacking but not working rn
            itemSlots[i].Item = startingItems[i];
        }
        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
        }
    }

    public bool AddItem(TEST_Item item)
    {
        if (IsFull())
        {
            return false;
        }
        startingItems.Add(item);
        SetStartingItems();
        return true;
    }

    public bool RemoveItem(TEST_Item item)
    {
        if (startingItems.Remove(item))
        {
            SetStartingItems();
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        return startingItems.Count >= itemSlots.Length;
    }

    // check if such item exists in the inventory
    public bool ContainsItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == item)
            {
                return true;
            }
        }
        return false;
    }

    public int ItemCount(string itemID)
    {
        int count = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item.ID == itemID)
            {
                count += itemSlots[i].Amount;
            }
        }
        return count;
    }

    // To remove the item with the specified ID and still keep a reference to it
    public TEST_Item RemoveItem(string itemID)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            TEST_Item item = itemSlots[i].Item;
            if (item != null && item.ID == itemID)
            {
                itemSlots[i].Item = null;
                return item;
            }
        }
        return null;
    }
}
