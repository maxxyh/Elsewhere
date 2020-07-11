using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedItemSlot : ItemSlot
{
    // public WeaponType weaponType;
    // public bool equipped;
    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = "Equipped Slot";
    }

    public override bool CanReceiveItem(Item item)
    {
        if (item == null)
        {
            return true;
        }
        EquippableItem equippableItem = item as EquippableItem;
        return equippableItem != null;
    }
}
