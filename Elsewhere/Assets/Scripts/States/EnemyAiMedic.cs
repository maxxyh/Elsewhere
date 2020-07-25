using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAiMedic : EnemyState
{
    public EnemyAiMedic(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    // try to heal the person, but if too far, screw it go for aggressive instead
    public override IEnumerator Execute()
    {
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);

        yield return new WaitForSecondsRealtime(0.75f);

        Tile targetTile = enemyUnit.medicTarget.currentTile;
        EnemyUnit medicTarget = enemyUnit.medicTarget;
        AStarSearch.GeneratePath(map, currUnit.currentTile, targetTile, false, true);
        
        // check if target tile is selectable, and also go as far from movement range as possible.
        int distanceFromTarget = 0, minHealingRange = (int) enemyUnit.teamHealingAbilities.Min(ability => ability.attackRange);
        while (!targetTile.selectable || distanceFromTarget < minHealingRange)
        {
            distanceFromTarget++;
            targetTile = targetTile.parent;
        }

        int maxHealingRange = (int) enemyUnit.teamHealingAbilities.Max(ability => ability.attackRange);

        // A star movement towards the target 
        currUnit.GetPathToTile(targetTile);

        yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);

        // check if there are players in range
        bool canHeal = false;
        foreach (Ability ability in enemyUnit.teamHealingAbilities) // TODO should be able to sort in ascending order by healing amount 
        {
            if (map.PlayerTargetInAttackRange(currUnit.currentTile, ability.attackRange, medicTarget))
            {
                currUnit.abilityTargetUnits = new List<Unit>() {medicTarget};  // TODO only supports single target healing at the moment
                currUnit.chosenAbility = ability;
                turnScheduler.SetState(new EnemyAbility(turnScheduler));
                canHeal = true;
            }
        }
        
        if (!canHeal)
        {
            turnScheduler.SetState(new WaitEnemyAI(turnScheduler));
        }
    }
}