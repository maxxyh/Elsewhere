using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityHPReaver : Ability
{
    public AbilityHPReaver() : base("HP Reaver", 3, 4, true, TargetingStyle.SINGLE)
    {
    }

    // Steals 40% of magic damage
    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach(Unit target in targets)
        {
            int magicDamage = BattleManager.CalculateMagicDamage(0.4f * initiator.stats["magicDamage"].Value, target);
            target.stats["HP"].AddModifier(new StatModifier(magicDamage, StatModType.Flat));
            initiator.stats["HP"].AddModifier(new StatModifier(magicDamage, StatModType.Flat));
            DamagePopUp.Create(target.transform.position, string.Format("- {0} HP", magicDamage), PopupType.DAMAGE);
            yield return new WaitForSecondsRealtime(0.5f);
            DamagePopUp.Create(initiator.transform.position, string.Format("+ {0}% HP", magicDamage), PopupType.HEAL);
        }

        base.Execute(initiator, targets);

        yield break;
    }
}
