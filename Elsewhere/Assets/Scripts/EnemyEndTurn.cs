using UnityEngine;
using System.Collections;

public class EnemyEndTurn : State
{
    public EnemyEndTurn(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        map.RemoveSelectedTiles(currUnit.currentTile);
        currUnit.EndTurn();

        // check whether there are still enemies in the queue -> if have then it should start the next enemies.
        if (turnScheduler.currTeamQueue.Count > 0)
        {
            turnScheduler.currTurn = Turn.ENEMY_TURN;
        }
        else
        {
            turnScheduler.currTurn = Turn.PLAYER_TURN;
        }
        turnScheduler.SetState(new Transition(turnScheduler));
        yield break;
    }

}