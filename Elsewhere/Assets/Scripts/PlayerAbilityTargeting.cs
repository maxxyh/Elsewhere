using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAbilityTargeting : State
{
    Ability ability;

    public PlayerAbilityTargeting(TurnScheduler turnScheduler) : base(turnScheduler)
    {
        ability = turnScheduler.currUnit.chosenAbility;
    }

    public override IEnumerator Execute()
    {
        map.RemoveSelectedTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, ability.attackRange, ability.targetingStyle);
        // display the attacking tiles.

        currUnit.currState = UnitState.TARGETING;
        yield break;
    }

    public override IEnumerator CheckTargeting(Tile tile)
    {
        if (!tile.attackable)
        {
            yield break;
        }

        // TODO Support clicking blank spaces and checking that there are players within the correct range?
        // TODO Support multi targeting

        #region Getting the correct target team
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
        #endregion


        List<Unit> targetUnits = new List<Unit>();


        #region Single Targeting Style

        if (ability.targetingStyle == TargetingStyle.SINGLE)
        {
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
                targetUnits.Add(targetUnit);
                currUnit.abilityTargetUnits = targetUnits;
                turnScheduler.confirmationPanel.SetActive(true);
            }
        }
        #endregion

        #region Radius Style
        if (ability.targetingStyle == TargetingStyle.RADIUS)
        {
            foreach(Unit unit in targetTeam)
            {
                if (map.GetAttackableTiles().Contains(unit.currentTile))
                {
                    targetUnits.Add(unit);
                }
            }

        }

        if (targetUnits.Count >0)
        {
            currUnit.abilityTargetUnits = targetUnits;
            turnScheduler.confirmationPanel.SetActive(true);
        }
        #endregion

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
