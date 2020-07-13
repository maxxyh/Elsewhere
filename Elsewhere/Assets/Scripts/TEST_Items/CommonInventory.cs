using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CommonInventory : ItemContainer
{
    [FormerlySerializedAs("items")]
    [SerializeField] protected List<Item> inventoryStartingItems;
    [SerializeField] protected Transform itemsParent;

    // public event Action<TEST_Item> OnItemRightClickedEvent;

    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);

        if (!Application.isPlaying)
        {
            SetStartingItems();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        SetStartingItems();
    }

    private void SetStartingItems()
    {
        Clear();
        foreach (Item item in inventoryStartingItems)
        {
            AddItem(item.GetCopy());
        }
    }
}
