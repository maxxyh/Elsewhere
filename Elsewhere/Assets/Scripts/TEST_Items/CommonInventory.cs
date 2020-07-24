using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CommonInventory : ItemContainer
{
    [SerializeField] protected List<Item> inventoryStartingItems;  // purely for convenient testing
    [SerializeField] protected Transform itemsParent;

    // public event Action<TEST_Item> OnItemRightClickedEvent;

    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);

        /*if (!Application.isPlaying)
        {
            SetStartingItems();
        }*/
    }

    protected override void Awake()
    {
        base.Awake();
        //SetStartingItems(inventoryStartingItems);
    }

    public void SetStartingItems(List<Item> inventoryStartingItems)
    {
        Clear();
        foreach (Item item in inventoryStartingItems)
        {
            AddItem(item.GetCopy());
        }
    }
}
