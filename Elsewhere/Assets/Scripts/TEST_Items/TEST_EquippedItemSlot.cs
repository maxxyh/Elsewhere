using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_EquippedItemSlot : TEST_ItemSlot
{
    // public WeaponType weaponType;
    // public bool equipped;
    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = "Equipped Slot";
    }

    public override bool CanReceiveItem(TEST_Item item)
    {
        if (item == null)
        {
            return true;
        }
        TEST_EquippableItem equippableItem = item as TEST_EquippableItem;
        return equippableItem != null;
    }
}
