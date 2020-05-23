using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    StatModifier mod1, mod2;

    public void Equip(Character c) 
    {
        mod1 = new StatModifier(10, StatModType.Flat, this);
        mod2 = new StatModifier(0.1f, StatModType.PercentMult, this);
        c.attackDamage.AddModifier(mod1);
        c.attackDamage.AddModifier(mod2);
    }

    public void UnEquip(Character c) 
    {
        c.attackDamage.RemoveAllModifiersFromSource(this);
    }
}
