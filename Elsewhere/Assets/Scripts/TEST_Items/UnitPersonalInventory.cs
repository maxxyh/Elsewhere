using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPersonalInventory : ItemContainer
{
    [SerializeField] protected List<Item> unitStartingItems;
    [SerializeField] protected Transform itemsParent;

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
        foreach (Item item in unitStartingItems)
        {
            AddItem(item.GetCopy());
        }
    }
}
