using UnityEngine;
using System.Collections;

public class PlayerEndTurn : State
{
    public PlayerEndTurn(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        map.RemoveSelectedTiles(currUnit.currentTile);
        currUnit.EndTurn();

        turnScheduler.playerActionPanel.SetActive(false);

        // check whether there are still players in the queue -> if have then it should start the next player.
        if (turnScheduler.currTeamQueue.Count > 0)
        {
            turnScheduler.currTurn = Turn.PLAYER_TURN;
        }
        else
        {
            turnScheduler.currTurn = Turn.ENEMY_TURN;
        }
        turnScheduler.SetState(new Transition(turnScheduler));
        yield break;
    }
}
