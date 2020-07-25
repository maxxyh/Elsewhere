using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/** responsibilities - mainly for recovery
 * iterate through all enemies and check for recovery, then assign medics
 */

public class EnemyAiManager
{
    public List<PlayerUnit> players;
    public List<EnemyUnit> enemies;
    public Map map;
    public List<EnemyUnit> medics;

    // TODO can also do the assignment of modes for each enemyUnit
    // Must be called only after all the enemies have been created 
    public EnemyAiManager(List<PlayerUnit> players, List<EnemyUnit> enemies, Map map)
    {
        this.players = players;
        this.enemies = enemies;
        this.map = map;
        medics = enemies.FindAll(unit => unit.teamHealingAbilities.Count > 0);
    }

    private void UpdateGameState(List<PlayerUnit> players, List<EnemyUnit> enemies, Map map)
    {
        this.players = players;
        this.enemies = enemies;
        this.map = map;
        medics = enemies.FindAll(unit => unit.teamHealingAbilities.Count > 0);
    }
    
    // decide order of movement
    // decide who is in recovery mode
    // decide who is going to heal those in recovery mode
    public LinkedList<Unit> StartTurn(List<PlayerUnit> players, List<EnemyUnit> enemies, Map map, LinkedList<Unit> currTeamQueue)
    {
        UpdateGameState(players, enemies, map);

        List<EnemyUnit> unitsInRecoveryMode = new List<EnemyUnit>();

        // Assign recovery mode
        foreach (EnemyUnit enemyUnit in enemies)
        {
            if (enemyUnit.IsRecoveryMode() && enemyUnit.selfHealingAbilities.Count == 0)
            {
                unitsInRecoveryMode.Add(enemyUnit);
            }
        }
        
        Queue<EnemyUnit> unassignedMedics = new Queue<EnemyUnit>(medics); 
        List<EnemyUnit> remainingHurtUnits = new List<EnemyUnit>(unitsInRecoveryMode);
        medics.ForEach(unit => unit.ResetMedicStatus());

        // Assign targets to medics = will keep running as long as there are there are both unassignedHurtUnits and unassignedMedics
        // TODO WRONG PERSON BEING ASSIGNED
        while (unitsInRecoveryMode.Count > 0 && unassignedMedics.Count > 0)
        {
            EnemyUnit currMedic = unassignedMedics.Dequeue();
            bool foundTarget = false;
            List<EnemyUnit> remainingHurtUnitsToSearch = new List<EnemyUnit>(unitsInRecoveryMode);
            
            while (!foundTarget && remainingHurtUnitsToSearch.Count > 0)
            {
                // Find closest target
                Tile targetTile = AStarSearch.GeneratePathToNearestTarget(map, currMedic.currentTile,
                    remainingHurtUnitsToSearch.ConvertAll(input => input.currentTile), false, true);
                EnemyUnit medicTarget = unitsInRecoveryMode.Find(unit => unit.currentTile == targetTile);

                // if already assigned to another medic, compare relative distance
                if (!remainingHurtUnits.Contains(medicTarget))
                {
                    EnemyUnit oldMedic = medics.Find(unit => unit.medicTarget == medicTarget);
                    if (targetTile.distance < oldMedic.distanceToMedicTarget)
                    {
                        currMedic.medicTarget = medicTarget;
                        currMedic.distanceToMedicTarget = targetTile.distance;
                        oldMedic.ResetMedicStatus();
                        unassignedMedics.Enqueue(oldMedic);
                        foundTarget = true;
                    }
                    else
                    {
                        remainingHurtUnitsToSearch.Remove(medicTarget);
                    }
                }
                
                // new target, has not been assigned a medic
                else
                {
                    currMedic.medicTarget = medicTarget;
                    currMedic.distanceToMedicTarget = targetTile.distance;
                    foundTarget = true;
                    remainingHurtUnits.Remove(medicTarget);
                }
            }
        }
        
        LinkedList<Unit> updatedCurrTeamQueue = new LinkedList<Unit>(currTeamQueue);
        // Reorder the queue: hurt people first, then medics
        foreach (Unit hurtUnit in unitsInRecoveryMode.FindAll(unit => !remainingHurtUnits.Contains(unit)))
        {
            updatedCurrTeamQueue.Remove(hurtUnit);
            updatedCurrTeamQueue.AddFirst(hurtUnit);
        }

        return updatedCurrTeamQueue;
    }

}
