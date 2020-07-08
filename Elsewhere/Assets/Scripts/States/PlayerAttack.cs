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

        // TODO how can like this?
        turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, currUnit.attackingTargetUnit));
        yield return new WaitUntil(() => currUnit.anim.GetBool("isAttacking") == false);

        currUnit.StartAttack(currUnit.attackingTargetUnit);
        BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);

        yield return new WaitForSeconds(1f);
        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
    }
}
