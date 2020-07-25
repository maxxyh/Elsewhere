using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityBloodShot : Ability
{
    public AbilityBloodShot() : base("Blood Shot", 3, 5, true, TargetingStyle.SELFSINGLE, new AbilityType[] { AbilityType.HEAL_SELF, AbilityType.HEAL_TEAM })
    {
    }

    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach(Unit target in targets)
        {
            int healingAmount = Mathf.CeilToInt(0.8f * initiator.stats[StatString.MAGIC_DAMAGE].Value);
            target.stats[StatString.HP].AddModifier(new StatModifier(healingAmount, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("+ {0} HP", healingAmount), PopupType.HEAL);
        }

        UpdateStats(initiator, targets);

        yield break;
    }
}