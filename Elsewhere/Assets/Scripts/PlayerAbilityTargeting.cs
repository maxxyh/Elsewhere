using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAbilityTargeting : State
{
    public PlayerAbilityTargeting(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        map.RemoveSelectedTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, turnScheduler.currUnit.chosenAbility.attackRange);
        // should display the attacking tiles.

        currUnit.currState = UnitState.TARGETING;
        yield break;
    }

    public override IEnumerator CheckTargeting(Tile tile)
    {
        Ability ability = turnScheduler.currUnit.chosenAbility;

        // TODO Support clicking blank spaces and checking that there are players within the correct range?
        // TODO Support multi targeting

        IEnumerable<Unit> targetTeam;
        if (turnScheduler.currTurn == Team.ENEMY)
        {
            if (ability.targetsSameTeam)
            {
                targetTeam = turnScheduler.enemies;
            }
            else
            {
                targetTeam = turnScheduler.players;
            }
        }
        else
        {
            if (ability.targetsSameTeam)
            {
                targetTeam = turnScheduler.players;
            }
            else
            {
                targetTeam = turnScheduler.enemies;
            }
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
            currUnit.abilityTargetUnit = targetUnit;
            turnScheduler.confirmationPanel.SetActive(true);
        }

        yield break;
    }


    public override IEnumerator Yes()
    {
        turnScheduler.confirmationPanel.SetActive(false);
        turnScheduler.SetState(new PlayerAbility(turnScheduler));
        yield break;
    }
    public override IEnumerator No()
    {
        turnScheduler.confirmationPanel.SetActive(false);
        yield break;
    }
}
