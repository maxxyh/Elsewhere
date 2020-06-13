using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityArcaneBoost : Ability
{
    public AbilityArcaneBoost() : base("Arcane Boost", 3, 4, true, TargetingStyle.SELF)
    {
    }

    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        foreach(Unit target in targets)
        {
            float magicDamageBuff = 0.2f;
            float armorBuff = 0.2f;
            target.stats["magicDamage"].AddModifier(new StatModifier(magicDamageBuff, StatModType.PercentAdd));
            target.stats["armor"].AddModifier(new StatModifier(armorBuff, StatModType.PercentAdd));
            DamagePopUp.Create(target.transform.position, string.Format("+ {0}% MagAttack", magicDamageBuff*100), PopupType.BUFF);
            yield return new WaitForSecondsRealtime(0.5f);
            DamagePopUp.Create(target.transform.position, string.Format("\n\n + {0}% Armor", armorBuff * 100), PopupType.BUFF);
        }

        base.Execute(initiator, targets);
        UpdateStats(initiator, targets);

        yield break;
    }
}
