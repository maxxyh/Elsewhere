using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AbilityDeadlyRicochet : Ability
{
    public AbilityDeadlyRicochet() : base("Deadly Ricochet", 4, 3, false,
        TargetingStyle.MULTI, new AbilityType[] {AbilityType.DAMAGE}, 2, 1)
    {
    }
    
    // Ranged AoE of 2-tile radius. Deals x1.5 physical damage to all targets in range
    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            int attackDamage = BattleManager.CalculateMagicDamage(1.5f * initiator.stats[StatString.PHYSICAL_DAMAGE].Value, target);
            target.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
        }
        UpdateStats(initiator, targets);

        yield break;
    }
}
