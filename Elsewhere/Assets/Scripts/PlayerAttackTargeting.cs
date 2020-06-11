using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class PlayerAttackTargeting : State
{
    public PlayerAttackTargeting(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        map.RemoveSelectedTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats["attackRange"].Value);
        // should display the attacking tiles.
        currUnit.currState = UnitState.TARGETING;

        yield break;

    }

    public override IEnumerator Attack()
    {
        turnScheduler.SetState(new PlayerAttack(turnScheduler));
        yield break;
    }

    public override IEnumerator CheckTargeting(Tile tile)
    {
        if (!tile.attackable)
        {
            yield break;
        }

        IEnumerable<Unit> targetTeam;

        if (turnScheduler.currTurn == Team.ENEMY)
        {
            targetTeam = turnScheduler.players;
        }
        else
        {
            targetTeam = turnScheduler.enemies;
        }

        Unit targetUnit = null;

        foreach (Unit unit in targetTeam)
        {
            if (unit.currentTile == tile)
            {
                targetUnit = unit;

            }
        }

        if (targetUnit != null)
        {
            turnScheduler.StartAttack(targetUnit);
        }
        
        yield break;
    }

}
