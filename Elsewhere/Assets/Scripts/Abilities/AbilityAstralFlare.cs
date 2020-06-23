using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class AbilityAstralFlare : Ability
{
    public AbilityAstralFlare() : base("Astral Flare", 3, 25, false, TargetingStyle.MULTI, 2, 4)
    {
    }

    // Does 110% magic damage to all units in range, and also reduces attack damage by 20%
    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            int attackDamage = BattleManager.CalculateMagicDamage(1.2f * initiator.stats[StatString.MAGIC_DAMAGE].Value, target);
            float armorDebuff = 0.2f;
            target.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            target.stats[StatString.ARMOR].AddModifier(new StatModifier(-armorDebuff, duration, StatModType.PercentAdd));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
            DamagePopUp.Create(target.transform.position, string.Format("\n\n- {0}% Attack", armorDebuff*100), PopupType.DEBUFF);
        }
        UpdateStats(initiator, targets);

        yield break;
    }
}
