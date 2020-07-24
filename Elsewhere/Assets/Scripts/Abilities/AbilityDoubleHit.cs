using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityDoubleHit : Ability
{
    public AbilityDoubleHit() : base("Double Hit", 1, 4, false, TargetingStyle.SINGLE, new AbilityType[] { AbilityType.DAMAGE })
    {
    }

    public override IEnumerator Execute(Unit attacker, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            float attackDamage = BattleManager.CalculateBaseDamage(attacker, target);
            target.stats[StatString.HP].AddModifier(new StatModifier(-2 * attackDamage, StatModType.Flat));
            
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
            yield return new WaitForSeconds(1f);
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
            
        }

        UpdateStats(attacker, targets);

        yield break;
    }
}
