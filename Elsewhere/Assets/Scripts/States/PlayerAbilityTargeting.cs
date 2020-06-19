using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class PlayerAbilityTargeting : State
{
    Ability ability;

    public PlayerAbilityTargeting(TurnScheduler turnScheduler) : base(turnScheduler)
    {
        ability = turnScheduler.currUnit.chosenAbility;
    }

    public override IEnumerator Execute()
    {
        turnScheduler.abilitiesPanel.SetActive(false);
        turnScheduler.cancelPanel.SetActive(true);

        map.FindAttackableTiles(currUnit.currentTile, ability.attackRange, ability.targetingStyle);
        // display the attacking tiles.

        currUnit.CurrState = UnitState.TARGETING;
        yield break;
    }

    public override IEnumerator CheckTargeting(Tile tile)
    {
        if (!tile.attackable)
        {
            yield break;
        }

        bool targetsFound = false;

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

        if (ability.targetingStyle == TargetingStyle.SINGLE || ability.targetingStyle == TargetingStyle.SELFSINGLE)
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
                targetsFound = true;
            }
        }
        #endregion

        #region Multi Targeting Style
        
        if (ability.targetingStyle == TargetingStyle.MULTI)
        {
            List<Tile> selectedTiles = new List<Tile>();

            // init BFS
            Queue<Tile> processing = new Queue<Tile>();

            // Compute Adjacency List and reset all distances.
            map.InitPathFinding(tile, false);
            selectedTiles.Add(tile);
            processing.Enqueue(tile);

            // relax edges with minimum SP estimate
            while (processing.Count > 0)
            {
                Tile node = processing.Dequeue();
                foreach (Tile neighbour in node.adjacencyList)
                {
                    if (neighbour.walkable && neighbour.distance > node.distance + 1 && node.distance + 1 <= ability.multiAbilityRange)
                    {
                        neighbour.distance = node.distance + 1;
                        selectedTiles.Add(neighbour);
                        processing.Enqueue(neighbour);
                    }
                }
            }

            foreach (Tile t in selectedTiles)
            {
                foreach (Unit unit in targetTeam)
                {
                    if (unit.currentTile == t)
                    {
                        targetUnits.Add(unit);
                    }
                }
            }

            if (targetUnits.Count > 0)
            {
                targetsFound = true;
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
            targetsFound = true;
        }
        #endregion

        #region Self Targeting Style
        if (ability.targetingStyle == TargetingStyle.SELF || ability.targetingStyle == TargetingStyle.SELFSINGLE)
        {
            if (tile == turnScheduler.currUnit.currentTile)
            {
                targetsFound = true;
                targetUnits.Add(turnScheduler.currUnit);
            }
        }

        #endregion

        if (targetsFound)
        {
            currUnit.abilityTargetUnits = targetUnits;
            turnScheduler.cancelPanel.SetActive(false);
            turnScheduler.confirmationPanel.SetActive(true);
            GameAssets.MyInstance.highlightMap.SetClicked();
        }

        yield break;
    }

    public override IEnumerator Yes()
    {
        turnScheduler.confirmationPanel.SetActive(false);
        turnScheduler.cancelPanel.SetActive(false);
        GameAssets.MyInstance.highlightMap.RemoveClicked();
        turnScheduler.SetState(new PlayerAbility(turnScheduler));
        yield break;
    }
    public override IEnumerator No()
    {
        turnScheduler.confirmationPanel.SetActive(false);
        turnScheduler.cancelPanel.SetActive(true);
        GameAssets.MyInstance.highlightMap.RemoveClicked();
        GameAssets.MyInstance.highlightMap.RemoveSelectedTiles();
        yield break;
    }

    public override IEnumerator Cancel()
    {
        turnScheduler.abilitiesPanel.SetActive(true);
        turnScheduler.cancelPanel.SetActive(false);
        map.RemoveAttackableTiles();

        turnScheduler.SetState(new PlayerAbilityMenu(turnScheduler));
        yield break;
    }
}
