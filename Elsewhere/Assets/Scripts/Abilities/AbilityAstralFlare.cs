using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class AbilityAstralFlare : Ability
{
    public AbilityAstralFlare() : base("Astral Flare", 3, 4, false, TargetingStyle.MULTI, 2)
    {
    }

    // Does 110% magic damage to all units in range, and also reduces attack damage by 20%
    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            int attackDamage = BattleManager.CalculateMagicDamage(1.2f * initiator.stats[StatString.MAGIC_DAMAGE].Value, target);
            float magicDebuff = 0.2f;
            target.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            target.stats[StatString.ATTACK_DAMAGE].AddModifier(new StatModifier(-magicDebuff, StatModType.PercentAdd));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
            DamagePopUp.Create(target.transform.position, string.Format("\n\n- {0}% Attack", magicDebuff), PopupType.DEBUFF);
        }
        UpdateStats(initiator, targets);

        yield break;
    }
}
