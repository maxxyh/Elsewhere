using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityHealingWave : Ability
{
    public AbilityHealingWave() : base("Healing Wave", 3, 4, true, TargetingStyle.SINGLE)
    {
    }

    public override IEnumerator Execute(List<Unit> targets)
    {
        foreach(Unit target in targets)
        {
            target.stats["HP"].AddModifier(new StatModifier(5, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, 5);
        }

        yield break;
    }
}
