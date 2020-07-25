using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAiAggressive : State
{
    public EnemyAiAggressive(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);

        yield return new WaitForSecondsRealtime(0.75f);

        List<Tile> targets = turnScheduler.players.ConvertAll(x => x.currentTile);
        Tile targetTile = AStarSearch.GeneratePathToNearestTarget(map, currUnit.currentTile, targets, false, true);
        Unit targetPlayer = turnScheduler.players.Find(x => x.currentTile == targetTile);

        // check if target tile is selectable, and also go as far from movement range as possible 
        /*int distanceFromTarget = 0, attackRange = (int) currUnit.stats[StatString.ATTACK_RANGE].Value;
        while (!targetTile.selectable || distanceFromTarget < attackRange)
        {
            distanceFromTarget++;
            targetTile = targetTile.parent;
        }*/
        while (!targetTile.selectable )
        {
            targetTile = targetTile.parent;
        }

        // A star movement towards the target 
        currUnit.GetPathToTile(targetTile);

        yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);

        // check if there are players in range
        if (map.PlayerTargetInAttackRange(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].Value, targetPlayer))
        {
            currUnit.attackingTargetUnit = targetPlayer;
            turnScheduler.SetState(new EnemyAttack(turnScheduler));
        }
        else
        {
            turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
        }
    }
}