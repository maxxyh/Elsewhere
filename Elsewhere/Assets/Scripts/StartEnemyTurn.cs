using UnityEngine;
using System.Collections;
using Packages.Rider.Editor.UnitTesting;

public class StartEnemyTurn : State
{

    public StartEnemyTurn(TurnScheduler turnScheduler) : base(turnScheduler)
    {

    }

    public override IEnumerator Execute()
    {

        // step 1: checking battlefield conditions then I choose a AI mode I want to use 
        // Step 1.5: Assign the AI type?
        // Step 2: AI.execute().

        // Attack  

        // Defence/Call for help - Buffs/Running Away 

        // Medic - When teammate nearby HP<30%  (needsHelp) -> can include -> target that person move to the person and heal the person. 

        // Capture


        //turnScheduler.SetState(...);

        turnScheduler.SetState(new AggressiveEnemyAI(turnScheduler));


        yield break;
    }

}
