using UnityEngine;
using System.Collections;

public class TutPlayerEndTurn : PlayerEndTurn
{
    public TutPlayerEndTurn(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        map.RemoveSelectableTiles(currUnit.currentTile);
        currUnit.EndTurn();
         
        turnScheduler.playerActionPanel.SetActive(false);
        // turnScheduler.currTeamQueue.Remove(currUnit);

        // check whether there are still players in the queue -> if have then it should start the next player.
        if (turnScheduler.currTeamQueue.Count > 0)
        {
            turnScheduler.currTurn = Team.PLAYER;
        }
        else
        {
            turnScheduler.currTurn = Team.ENEMY;
        }
        
        Debug.Log("Inside endturn curr team queue cout: " + turnScheduler.currTeamQueue.Count);
        turnScheduler.SetState(new TutTransition(turnScheduler));
        
        yield break;
    }
}
