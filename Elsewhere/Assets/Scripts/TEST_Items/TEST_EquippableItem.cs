using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Gun,
    Magic_Item,
}
[CreateAssetMenu(menuName = "Items / Equippable Item")]
public class TEST_EquippableItem : TEST_Item
{
    /*private Dictionary<StatString, int> weaponStats = new Dictionary<StatString, int>()
    {
        {StatString.MIGHT, 0 },
        {StatString.HIT_RATE, 30},
        {StatString.CRIT_RATE, 0},
        {StatString.ATTACK_RANGE, 1}
    };*/
    public int physicalAttackBonus;
    public int magicalAttackBonus;
    public int attackRange;
    public int critBonus;
    public int hitRate;
    /*[Space]
    public float physicalAttackPercentBonus;
    public float magicalAttackPercentBonus;
    public float attackRangePercentBonus;*/
    [Space]
    public WeaponType weaponType;

    public override TEST_Item GetCopy()
    {
        return Instantiate(this);
    }

    public override void Destroy()
    {
        Destroy(this);
    }

    public void Equip(TEST_Unit inventoryManager)
    {
        if (physicalAttackBonus != 0)
            inventoryManager.physicalAttack.AddModifier(new StatModifier(physicalAttackBonus, StatModType.Flat, this));
        if (magicalAttackBonus != 0)
            inventoryManager.magicalAttack.AddModifier(new StatModifier(magicalAttackBonus, StatModType.Flat, this));
        if (physicalAttackBonus != 0)
            inventoryManager.attackRange.AddModifier(new StatModifier(attackRange, StatModType.Flat, this));
        if (physicalAttackBonus != 0)
            inventoryManager.critRate.AddModifier(new StatModifier(critBonus, StatModType.Flat, this));
        if (physicalAttackBonus != 0)
            inventoryManager.hitRate.AddModifier(new StatModifier(hitRate, StatModType.Flat, this));
    }

    public void Unequip(TEST_Unit inventoryManager)
    {
        inventoryManager.physicalAttack.RemoveAllModifiersFromSource(this);
        inventoryManager.magicalAttack.RemoveAllModifiersFromSource(this);
        inventoryManager.attackRange.RemoveAllModifiersFromSource(this);
        inventoryManager.critRate.RemoveAllModifiersFromSource(this);
        inventoryManager.hitRate.RemoveAllModifiersFromSource(this);
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
