using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityWhirlwindSlash : Ability
{
    public AbilityWhirlwindSlash() : base("Whirlwind Slash", 1, 4, false, TargetingStyle.RADIUS)
    {
    }

    public override IEnumerator Execute(List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.stats["HP"].AddModifier(new StatModifier(-5, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, 5);
        }

        yield break;
    }
}
