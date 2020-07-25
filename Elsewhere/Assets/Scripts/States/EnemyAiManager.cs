using System.Collections.Generic;


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
    public EnemyAiManager(TurnScheduler turnScheduler)
    {
        players = turnScheduler.players;
        enemies = turnScheduler.enemies;
        map = turnScheduler.map;
        medics = enemies.FindAll(unit => unit.teamHealingAbilities.Count > 0);
    }

    private void UpdateGameState(TurnScheduler turnScheduler)
    {
        players = turnScheduler.players;
        enemies = turnScheduler.enemies;
        map = turnScheduler.map;
    }
    
    // decide order of movement
    // decide who is in recovery mode
    // decide who is going to heal those in recovery mode
    public void StartTurn(TurnScheduler turnScheduler)
    {
        UpdateGameState(turnScheduler);

        List<EnemyUnit> unitsInRecoveryMode = new List<EnemyUnit>();

        // Assign recovery mode
        foreach (EnemyUnit enemyUnit in enemies)
        {
            enemyUnit.CheckIfRecoveryMode();
            if (enemyUnit.inRecoveryMode && enemyUnit.selfHealingAbilities.Count == 0)
            {
                unitsInRecoveryMode.Add(enemyUnit);
            }
        }
        
        Queue<EnemyUnit> unassignedMedics = new Queue<EnemyUnit>(medics); 
        List<EnemyUnit> remainingHurtUnits = new List<EnemyUnit>(unitsInRecoveryMode);
        medics.ForEach(unit => unit.ResetMedicStatus());

        // Assign targets to medics = will keep running as long as there are there are both unassignedHurtUnits and unassignedMedics
        while (remainingHurtUnits.Count > 0 && unassignedMedics.Count > 0)
        {
            EnemyUnit currMedic = unassignedMedics.Dequeue();
            bool foundTarget = false;
            List<EnemyUnit> remainingHurtUnitsToSearch = new List<EnemyUnit>(unitsInRecoveryMode);
            
            while (!foundTarget)
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
        
        
        // Reorder the queue: hurt people first, then medics
        foreach (Unit hurtUnit in unitsInRecoveryMode.FindAll(unit => !remainingHurtUnits.Contains(unit)))
        {
            turnScheduler.currTeamQueue.Remove(hurtUnit);
            turnScheduler.currTeamQueue.AddFirst(hurtUnit);
        }
    }

}
