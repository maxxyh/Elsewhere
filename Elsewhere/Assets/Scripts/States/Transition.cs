/** State between Enemy and Player Turn
 * Checks if there are still players alive on each team. If there are, it continues with the turn provided.
 */

using UnityEngine;
using System.Collections;
using System.Linq;

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
            if (turnScheduler.objectiveType == ObjectiveType.ELIMINATE_ALL_ENEMIES)
            {
                turnScheduler.objectiveCompleted = true;
            }
        }

        if (turnScheduler.objectiveCompleted)
        {
            turnScheduler.SetState(new Win(turnScheduler));
            yield break;
        }

        // check whether there are still players in the queue -> if have then it should start the next unit in same team
        // else will switch team and enqueue.
        // use of while loop to enable movement after all members of 1 team have been eliminated.
        while (turnScheduler.currTeamQueue.Count == 0)
        {
            if (turnScheduler.currTurn == Team.PLAYER)
            {
                turnScheduler.currTurn = Team.ENEMY;
                turnScheduler.enemyPhasePanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                turnScheduler.enemyPhasePanel.SetActive(false);
                foreach (Unit player in turnScheduler.players) player.RemoveGrayscale();
            }
            else if (turnScheduler.currTurn == Team.ENEMY)
            {
                turnScheduler.currTurn = Team.PLAYER;
                turnScheduler.playerPhasePanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                turnScheduler.playerPhasePanel.SetActive(false);
                foreach (Unit enemy in turnScheduler.enemies) enemy.RemoveGrayscale();
            }
            turnScheduler.EnqueueTeams(turnScheduler.currTurn);
        }


        // update current unit
        turnScheduler.currUnit = turnScheduler.currTeamQueue.First();


        // start next turn/next unit
        if (turnScheduler.currTurn == Team.ENEMY)
        {
            turnScheduler.SetState(new EnemyStartTurn(turnScheduler));
        }
        else
        {
            turnScheduler.SetState(new PlayerStartTurn(turnScheduler));
        }
        yield break;
    }
}
