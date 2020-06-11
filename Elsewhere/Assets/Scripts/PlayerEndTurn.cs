using UnityEngine;
using System.Collections;

public class PlayerEndTurn : State
{
    public PlayerEndTurn(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        map.RemoveSelectableTiles(currUnit.currentTile);
        currUnit.EndTurn();

        turnScheduler.playerActionPanel.SetActive(false);

        // check whether there are still players in the queue -> if have then it should start the next player.
        if (turnScheduler.currTeamQueue.Count > 0)
        {
            turnScheduler.currTurn = Team.PLAYER;
        }
        else
        {
            turnScheduler.currTurn = Team.ENEMY;
        }
        turnScheduler.SetState(new Transition(turnScheduler));
        yield break;
    }
}
