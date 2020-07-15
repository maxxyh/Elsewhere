using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPersonalInventory : ItemContainer
{
    [SerializeField] protected Transform itemsParent;

    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);

    }

}
