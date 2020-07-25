/** Description of AI:
 * If able to heal, retreat from enemies and use item
 * Internal state turns to "help" mode →
 * Retreat towards an allied healer(only if unit can stay out of enemy range)
 * Retreat to nearest fort(only if unit can stay out of enemy range)
*/

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class EnemyAttack : EnemyState
{
    public EnemyAttack(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        Unit targetPlayer = currUnit.attackingTargetUnit;

        map.RemoveSelectableTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].Value);
        // should display the attacking tiles.

        yield return new WaitForSecondsRealtime(1);

        map.RemoveAttackableTiles();

        // TODO how can like this?
        turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, targetPlayer));
        yield return new WaitUntil(() => currUnit.anim.GetBool("isAttacking") == false);


        currUnit.StartAttack(targetPlayer);
        BattleManager.Battle(currUnit, targetPlayer);

        yield return new WaitForSeconds(1f);
        turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
    }
}

