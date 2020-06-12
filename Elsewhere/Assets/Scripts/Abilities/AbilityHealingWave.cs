using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityHealingWave : Ability
{
    public AbilityHealingWave() : base("Healing Wave", 3, 4, true, TargetingStyle.SELFSINGLE)
    {
    }

    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach(Unit target in targets)
        {
            int healingAmount = 5;
            target.stats["HP"].AddModifier(new StatModifier(healingAmount, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("+ {0} HP", healingAmount), PopupType.HEAL);
        }

        base.Execute(initiator, targets);

        yield break;
    }
}
