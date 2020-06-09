using UnityEngine;
using System.Collections;
public class AggressiveEnemyAI : State
{
    public AggressiveEnemyAI(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].baseValue);

        yield return new WaitForSecondsRealtime(0.75f);

        // use distance to determine closest player
        int minDistance = int.MaxValue;
        Unit targetPlayer = turnScheduler.players[0];
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
        Debug.Log(targetTile.transform.position);
        AStarSearch.GeneratePath(map, currUnit.currentTile, targetPlayer.currentTile, false, true);

        // check if target tile is selectable 
        while (!targetTile.selectable)
        {
            targetTile = targetTile.parent;
        }

        /*
        // get target tile by subtracting the attackRange
        int attackRange = (int) currUnit.stats["attackRange"].baseValue;
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

        yield return new WaitUntil(() => currUnit.currState == UnitState.IDLING);

        // check if there are players in range
        if (map.PlayerTargetInRange(currUnit.currentTile, currUnit.stats["attackRange"].baseValue, targetPlayer))
        {
            currUnit.attackingTargetUnit = targetPlayer;
            turnScheduler.OnAttackButton();
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

        map.RemoveSelectedTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats["attackRange"].baseValue);
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