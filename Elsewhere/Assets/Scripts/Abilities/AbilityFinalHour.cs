using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFinalHour : Ability
{
    public AbilityFinalHour() : base("Final Hour", 10, 8, false, TargetingStyle.SINGLE, new [] {AbilityType.DAMAGE})
    {}

    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            int heuristicDistance =
                Mathf.CeilToInt(Vector3.Distance(initiator.transform.position, target.transform.position));
            
            var attackDamage = Mathf.CeilToInt((1 + (heuristicDistance - 1) * 0.2f) * BattleManager.CalculatePhysicalDamage(initiator.stats[StatString.PHYSICAL_DAMAGE].Value, target));
            
            target.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);
            
        }

        UpdateStats(initiator, targets);

        yield break;
    }
}