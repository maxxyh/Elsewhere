using UnityEngine;
using System.Collections;

public class PlayerAttack : State
{
    public PlayerAttack(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        map.RemoveAttackableTiles();

        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(currUnit.attackingTargetUnit);
        BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);

        // TODO how can like this?
        yield return turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, currUnit.attackingTargetUnit));

        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
    }
}
