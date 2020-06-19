﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class AggressiveEnemyAI : State
{
    public AggressiveEnemyAI(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);

        yield return new WaitForSecondsRealtime(0.75f);

        // use distance to determine closest player
        int minDistance = int.MaxValue;
        Unit targetPlayer = turnScheduler.players.ElementAt(0);
        foreach (Unit player in turnScheduler.players)
        {
            AStarSearch.GeneratePath(map, currUnit.currentTile, player.currentTile, false, true);
            if (player.currentTile.distance < minDistance)
            {
                minDistance = player.currentTile.distance;
                targetPlayer = player;
            }
        }

        Tile targetTile = targetPlayer.currentTile;
        AStarSearch.GeneratePath(map, currUnit.currentTile, targetPlayer.currentTile, false, true);

        // check if target tile is selectable 
        while (!targetTile.selectable)
        {
            targetTile = targetTile.parent;
        }

        /*
        // get target tile by subtracting the attackRange
        int attackRange = (int) currUnit.stats["attackRange"].Value;
        for (int i = 0; i < attackRange; i++)
        {
            if (targetTile == currUnit.currentTile)
            {
                break;
            }
            else
            {
                targetTile = targetTile.parent;
            }                
        }
        */

        // A star movement towards the target 
        currUnit.GetPathToTile(targetTile);

        yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);

        // check if there are players in range
        if (map.PlayerTargetInRange(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].Value, targetPlayer))
        {
            currUnit.attackingTargetUnit = targetPlayer;
            turnScheduler.StartAttack(targetPlayer);
        }
        else
        {
            turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
        }


        yield break;
    }

    public override IEnumerator Attack()
    {
        Unit targetPlayer = currUnit.attackingTargetUnit;

        map.RemoveSelectableTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].Value) ;
        // should display the attacking tiles.

        yield return new WaitForSecondsRealtime(1);

        map.RemoveAttackableTiles();
        currUnit.StartAttack(targetPlayer);
        BattleManager.Battle(currUnit, targetPlayer);

        // TODO how can like this?
        yield return turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, targetPlayer));

        turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
    }

}