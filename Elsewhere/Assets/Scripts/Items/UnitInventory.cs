using UnityEngine;

[CreateAssetMenu(fileName ="Unit Inventory", menuName = "ScriptableObjects/UnitInventory")]
public class UnitInventory : Inventory
{

    public override void UseItem(Item item)
    {
        if (item is IEquippable)
        {
            (item as IEquippable).UseWeapon();
        } 
        else if (item is IUseable)
        {
            (item as IUseable).Use(unit.stats);
        }
    }

    public override void EquipItem(Item item)
    {
        if (item is IEquippable)
        {
            unit.weapon = item as Weapon;
            (item as IEquippable).Equip();
        }
    }

    public override void UnequipItem(Item item)
    {
        if (item is IEquippable)
        {
            unit.weapon = null;
            (item as IEquippable).UnEquip();
        }
    }
}