using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityDefault : Ability
{
    public AbilityDefault() : base("Default Attack", 2, 1, false, TargetingStyle.SINGLE, new AbilityType[] { AbilityType.DAMAGE } )
    {
    }

    public override IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        return base.Execute(initiator, targets);
    }
}
