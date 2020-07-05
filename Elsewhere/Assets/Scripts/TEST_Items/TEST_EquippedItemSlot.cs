using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_EquippedItemSlot : TEST_ItemSlot
{
    // public WeaponType weaponType;
    public bool equipped;
    public override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = "Equipped Slot";
    }
}
