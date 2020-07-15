using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Usable Item Effects / Heal")]
public class HealthItemEffect : UsableItemEffect
{
    public int healAmount;
    public override void ExecuteEffect(UsableItem parentItem, Unit unit)
    {
        unit.stats[StatString.HP].AddModifier(new StatModifier(healAmount, StatModType.Flat));
    }

    public override string GetDescription()
    {
        return "Heal for +" + healAmount + " HP";
    }
}
