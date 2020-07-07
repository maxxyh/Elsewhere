using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour, IItemContainer
{
    public List<TEST_ItemSlot> ItemSlots;
    public event Action<TEST_BaseItemSlot> OnPointerEnterEvent;
    public event Action<TEST_BaseItemSlot> OnPointerExitEvent;
    public event Action<TEST_BaseItemSlot> OnRightClickEvent;

    protected virtual void OnValidate()
    {
        GetComponentsInChildren(includeInactive: true, result: ItemSlots);
    }

    protected virtual void Awake()
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            ItemSlots[i].OnPointerEnterEvent += slot => EventHelper(slot, OnPointerEnterEvent);
            ItemSlots[i].OnPointerExitEvent += slot => EventHelper(slot, OnPointerExitEvent);
            ItemSlots[i].OnRightClickEvent += slot => EventHelper(slot, OnRightClickEvent);
            /*ItemSlots[i].OnBeginDragEvent += slot => EventHelper(slot, OnBeginDragEvent);
            ItemSlots[i].OnEndDragEvent += slot => EventHelper(slot, OnEndDragEvent);
            ItemSlots[i].OnDragEvent += slot => EventHelper(slot, OnDragEvent);
            ItemSlots[i].OnDropEvent += slot => EventHelper(slot, OnDropEvent);*/
        }
    }

    private void EventHelper(TEST_BaseItemSlot itemSlot, Action<TEST_BaseItemSlot> action)
    {
        action?.Invoke(itemSlot);
    }

    public virtual bool AddItem(TEST_Item item)
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].CanAddStack(item))
            {
                ItemSlots[i].Item = item;
                ItemSlots[i].Amount++;
                return true;
            }
        }

        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item == null)
            {
                ItemSlots[i].Item = item;
                ItemSlots[i].Amount++;
                return true;
            }
        }
        return false;
    }

    public virtual bool CanAddItem(TEST_Item item, int amount = 1)
    {
        int freeSpaces = 0;

        foreach (TEST_ItemSlot itemSlot in ItemSlots)
        {
            if (itemSlot.Item == null || itemSlot.Item.ID == item.ID)
            {
                freeSpaces += item.maxStack - itemSlot.Amount;
            }
        }
        return freeSpaces >= amount;
    }

    public virtual void Clear()
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item != null && Application.isPlaying)
            {
                ItemSlots[i].Item.Destroy();
            }
            ItemSlots[i].Item = null;
            ItemSlots[i].Amount = 0;
        }
    }

    public virtual int ItemCount(string itemID)
    {
        int number = 0;

        for (int i = 0; i < ItemSlots.Count; i++)
        {
            TEST_Item item = ItemSlots[i].Item;
            if (item != null && item.ID == itemID)
            {
                number += ItemSlots[i].Amount;
            }
        }
        return number;
    }

    public virtual TEST_Item RemoveItem(string itemID)
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            TEST_Item item = ItemSlots[i].Item;
            if (item != null && item.ID == itemID)
            {
                ItemSlots[i].Amount--;
                return item;
            }
        }
        return null;
    }

    public virtual bool RemoveItem(TEST_Item item)
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item == item)
            {
                ItemSlots[i].Amount--;
                return true;
            }
        }
        return false;
    }
}
