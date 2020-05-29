    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : Action
{
    public AttackAction(int actionID, Unit currUnit) : base(actionID, currUnit) {
    }
    public override Action GenerateNextAction() 
    {
        /*
        while (true)
        {
            if (currUnit.isAttacking)
            {
                BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);
                return new EndAction(actionID + 1, currUnit);
            }
        }
        */
        StartCoroutine(currUnit.CheckForAttacking());
        BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);
        return new EndAction(actionID + 1, currUnit);

    }
}
