using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    StatModifier mod1, mod2;

    public void Equip(PlayerUnit c) 
    {
        mod1 = new StatModifier(10, StatModType.Flat, this);
        mod2 = new StatModifier(0.1f, StatModType.PercentMult, this);
        c.stats[StatString.ATTACK_DAMAGE].AddModifier(mod1);
        c.stats[StatString.ATTACK_DAMAGE].AddModifier(mod2);
    }

    public void UnEquip(PlayerUnit c) 
    {
        c.stats[StatString.ATTACK_DAMAGE].RemoveAllModifiersFromSource(this);
    }
}
