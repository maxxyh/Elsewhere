using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;
using System.IO.IsolatedStorage;

public class EnemyStartTurn : EnemyState
{

    public EnemyStartTurn(TurnScheduler turnScheduler) : base(turnScheduler)
    {
        
    }

    public override IEnumerator Execute()
    {
        /* AI dependencies: 
         * Map 
         * Units in turnScheduler
         */

        // step 0: Each enemy should have some AI that is available to it?
        // step 1: checking battlefield conditions then I choose a AI mode I want to use AI.AnalyzeField(CurrUnit) which will return an AI Type - this is like a recommendation system?
        // Step 1.5: Assign the AI type?
        // Step 2: AI.execute().

        // Attack 

        // Defence/Call for help - Buffs/Running Away 

        // Medic - When teammate nearby HP<30%  (needsHelp) -> can include -> target that person move to the person and heal the person. 

        // Capture

        // EnemyUnit can have states in the update function that check the health periodically to see if they're in need of healing?


        //turnScheduler.SetState(...);

        // Recovery Mode floats are determined by predefined personalities of the unit. Let's fix it for now.
        
        // if it's the first enemy, let enemyAiManager check the battlefield.
        if (turnScheduler.currTeamQueue.Count == turnScheduler.enemies.Count)
        {
            turnScheduler.currTeamQueue = turnScheduler.enemyAiManager.StartTurn(turnScheduler.players,
                turnScheduler.enemies, map, turnScheduler.currTeamQueue);
            
            // fixing the reference
            turnScheduler.currUnit = turnScheduler.currTeamQueue.First();
            currUnit = turnScheduler.currUnit;
            enemyUnit = turnScheduler.enemies.Find(x => x.currentTile == currUnit.currentTile);
        }
        
        turnScheduler.currTeamQueue.RemoveFirst();
        currUnit.StartTurn();

        bool recovery = enemyUnit.IsRecoveryMode();
        Debug.Log($"Number {enemies.Count - turnScheduler.currTeamQueue.Count}, position = {currUnit.transform.position}, recovery = {recovery}");
        
        if (currUnit.isStunned)
        {
            yield return currUnit.StunAnimation();
            yield return new WaitForSeconds(1f);
            turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
        }
        // prioritize self preservation over saving others
        else if (recovery)
        {
            Debug.Log("going into recovery mode");
            turnScheduler.SetState(new EnemyAiRecovery(turnScheduler));
        }
        
        else if (enemyUnit.IsMedic())
        {
            turnScheduler.SetState(new EnemyAiMedic(turnScheduler));
        }
        
        else if (enemyUnit.HasWaitingMode())
        {
            turnScheduler.SetState(new EnemyAiWait(turnScheduler));
        }
        
        else
        {
            turnScheduler.SetState(new EnemyAiAggressive(turnScheduler));
        }
    }

}
