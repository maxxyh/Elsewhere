using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AbilityIonicStrike : Ability
{
    public AbilityIonicStrike() : base("Ionic Strike", 3, 5, false, TargetingStyle.SINGLE, new[] {AbilityType.DAMAGE},
        0, 2)
    {
        
    }

    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            bool stun = new Random().Next(1,100) <= 99;
            
            var attackDamage = BattleManager.CalculatePhysicalDamage(0.8f * initiator.stats[StatString.PHYSICAL_DAMAGE].Value, target);
            target.stats[StatString.HP].AddModifier(new StatModifier(-attackDamage, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", attackDamage), PopupType.DAMAGE);

            if (stun)
            {
                yield return new WaitForSeconds(1f);
                Debug.Log("target stunned");
                target.Stun();
                yield return target.StunAnimation();
                DamagePopUp.Create(target.transform.position, "Stunned!", PopupType.DAMAGE);
            }
        }
        UpdateStats(initiator, targets);

        yield break;
    }
}
