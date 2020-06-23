using UnityEngine;
using System.Collections;
using System.Linq;

public class TutTransition : Transition
{
    public TutTransition(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        Debug.Log("transition tutturn = " + turnScheduler.TutTurn);
        //setting the correct dialogue
        if (turnScheduler.TutTurn == 1)
        {
            turnScheduler.TutTurn++;
            turnScheduler.nextChooseDialogue.SetActive(true);
        }
        else if (turnScheduler.TutTurn == 0)
        { 
            turnScheduler.TutTurn = 1;
        }
        else 
        {
            turnScheduler.startEnemyTurnDialogue.SetActive(true);
        }

        // enqueue teams 
        Debug.Log("count: " + turnScheduler.currTeamQueue.Count);
        if (turnScheduler.currTeamQueue.Count == 0)
        {
            if (turnScheduler.currTurn == Team.ENEMY)
            {
                turnScheduler.enemyPhasePanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                turnScheduler.enemyPhasePanel.SetActive(false);
                foreach (Unit player in turnScheduler.players) player.RemoveGrayscale();
            }
            else if (turnScheduler.currTurn == Team.PLAYER)
            {
                turnScheduler.playerPhasePanel.SetActive(true);
                yield return new WaitForSeconds(1f);
                turnScheduler.playerPhasePanel.SetActive(false);
                foreach (Unit enemy in turnScheduler.enemies) enemy.RemoveGrayscale();
            }
            turnScheduler.EnqueueTeams(turnScheduler.currTurn);
        }
        turnScheduler.currUnit = turnScheduler.currTeamQueue.First();

        // start next turn/next unit
        if (turnScheduler.currTurn == Team.ENEMY)
        {
            turnScheduler.currTeamQueue.RemoveFirst();
            turnScheduler.SetState(new EnemyStartTurn(turnScheduler));
        }
        else
        {
            turnScheduler.SetState(new TutPlayerStartTurn(turnScheduler));
        }
        yield break;
    }
}
