using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityDoubleHit : Ability
{
    public AbilityDoubleHit() : base("Double Hit", 1, 4, false, TargetingStyle.SINGLE)
    {
    }

    public override IEnumerator Execute(Unit attacker, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            float attackDamage = BattleManager.CalculateBaseDamage(attacker, target);
            for (int i = 0; i < 2; i++)
            {
                target.stats["HP"].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
                DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
            }
        }
        base.Execute(attacker, targets);
        yield break;
    }
}
