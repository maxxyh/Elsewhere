using UnityEngine;
using System.Collections;

public class TutPlayerAttack : PlayerAttack
{
    public TutPlayerAttack(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.attackDialogue.SetActive(false);
        map.RemoveAttackableTiles();

        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(currUnit.attackingTargetUnit);
        BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);

        // TODO how can like thi s?
        yield return turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, currUnit.attackingTargetUnit));
        
        turnScheduler.SetState(new TutPlayerEndTurn(turnScheduler));
    }
}
