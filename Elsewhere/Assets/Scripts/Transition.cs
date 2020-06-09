/** State between Enemy and Player Turn
 * Checks if there are still players alive on each team. If there are, it continues with the turn provided.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.XR.WSA.Input;
using UnityEngine.SocialPlatforms;

public class Transition : State
{
    public Transition(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        // check if game has ended.
        if (turnScheduler.players.Count == 0)
        {
            turnScheduler.SetState(new Lose(turnScheduler));
            yield break;
        }
        else if (turnScheduler.enemies.Count == 0)
        {
            turnScheduler.SetState(new Win(turnScheduler));
            yield break;
        }

        // continue game
        // check if need to initiatlize team
        if (turnScheduler.currTeamQueue.Count == 0)
        {
            turnScheduler.EnqueueTeams(turnScheduler.currTurn);
        }
        // update current unit
        turnScheduler.currUnit = turnScheduler.currTeamQueue.Dequeue();

        
        // start next turn
        if (turnScheduler.currTurn == Team.ENEMY)
        {
            turnScheduler.SetState(new StartEnemyTurn(turnScheduler));
        }
        else
        {
            turnScheduler.SetState(new StartPlayerTurn(turnScheduler));
        }

        yield break;
    }
}
