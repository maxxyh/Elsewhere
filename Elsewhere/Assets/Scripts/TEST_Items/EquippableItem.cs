using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Sword,
    Gun,
    Magic,
    Protection,
    Consumable
}
[CreateAssetMenu(menuName = "Items / Equippable Item")]
public class EquippableItem : Item
{
    public int physicalAttackBonus;
    public int magicalAttackBonus;
    public int attackRange;
    public int critBonus;
    public int hitRate;
    
    [Space]
    public ItemType weaponType;

    public override Item GetCopy()
    {
        return Instantiate(this);
    }

    public override void Destroy()
    {
        Destroy(this);
    }

    public void Equip(InBattleUnitInventoryManager inventoryManager)
    {
        if (physicalAttackBonus != 0)
            inventoryManager.unit.stats[StatString.PHYSICAL_DAMAGE].AddModifier(new StatModifier(physicalAttackBonus, StatModType.Flat, this));
        if (magicalAttackBonus != 0)
            inventoryManager.unit.stats[StatString.MAGIC_DAMAGE].AddModifier(new StatModifier(magicalAttackBonus, StatModType.Flat, this));
        if (physicalAttackBonus != 0)
            inventoryManager.unit.stats[StatString.ATTACK_RANGE].AddModifier(new StatModifier(attackRange, StatModType.Flat, this));
        if (physicalAttackBonus != 0)
            inventoryManager.unit.stats[StatString.CRIT_RATE].AddModifier(new StatModifier(critBonus, StatModType.Flat, this));
        if (physicalAttackBonus != 0)
            inventoryManager.unit.stats[StatString.HIT_RATE].AddModifier(new StatModifier(hitRate, StatModType.Flat, this));
    }

    public void Unequip(InBattleUnitInventoryManager inventoryManager)
    {
        inventoryManager.unit.stats[StatString.PHYSICAL_DAMAGE].RemoveAllModifiersFromSource(this);
        inventoryManager.unit.stats[StatString.MAGIC_DAMAGE].RemoveAllModifiersFromSource(this);
        inventoryManager.unit.stats[StatString.ATTACK_RANGE].RemoveAllModifiersFromSource(this);
        inventoryManager.unit.stats[StatString.CRIT_RATE].RemoveAllModifiersFromSource(this);
        inventoryManager.unit.stats[StatString.HIT_RATE].RemoveAllModifiersFromSource(this);
    }

    public override string GetItemType()
    {
        return weaponType.ToString();
    }

    public override string GetDescription()
    {
        sb.Length = 0;

        AddStat(physicalAttackBonus, "Physical Attack");
        AddStat(magicalAttackBonus, "Magical Attack");
        AddStat(hitRate, "Hit Rate");
        AddStat(critBonus, "Crit Rate");
        AddStat(attackRange, "Attack Range");
        AddStat(itemNumUses, "Durability");
        return sb.ToString();
    }

    private void AddStat(float value, string statName)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if (value > 0)
            {
                sb.Append("+");
            }
            sb.Append(value);
            sb.Append(" ");
            sb.Append(statName);
        }
    }
}
