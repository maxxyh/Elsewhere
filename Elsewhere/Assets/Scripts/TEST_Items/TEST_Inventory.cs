using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TEST_Inventory : ItemContainer
{
    [FormerlySerializedAs("items")]
    [SerializeField] protected List<TEST_Item> startingItems;
    [SerializeField] protected Transform itemsParent;

    public event Action<TEST_Item> OnItemRightClickedEvent;

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
        Debug.Log("Set starting Item");
        Clear();
        foreach (TEST_Item item in startingItems)
        {
            AddItem(item.GetCopy());
        }
    }
}
