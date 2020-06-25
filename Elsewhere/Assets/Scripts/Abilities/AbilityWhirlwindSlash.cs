using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class AbilityWhirlwindSlash : Ability
{
    public AbilityWhirlwindSlash() : base("Whirlwind Slash", 1, 20, false, TargetingStyle.RADIUS, new AbilityType[] { AbilityType.DAMAGE })
    {
    }

    // Does 120% damage to all units around a 1 tile radius
    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            int attackDamage = BattleManager.CalculatePhysicalDamage(1.2f * initiator.stats[StatString.PHYSICAL_DAMAGE].Value, target);
            target.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
        }
        UpdateStats(initiator, targets);

        yield break;
    }
}
