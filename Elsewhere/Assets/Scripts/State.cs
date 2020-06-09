using UnityEngine;
using System.Collections;

public abstract class State
{

    protected TurnScheduler turnScheduler;
    protected Map map;
    protected Unit currUnit;

    public State(TurnScheduler turnScheduler)
    {
        this.turnScheduler = turnScheduler;
        this.map = turnScheduler.map;
        this.currUnit = turnScheduler.currUnit;
    }

    public virtual IEnumerator Execute()
    {
        yield break;
    }

    public virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator EndTurn()
    {
        if (currUnit.currState == UnitState.IDLING)
        {
            if (turnScheduler.currTurn == Turn.PLAYER_TURN)
            {
                turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
            } 
            else
            {
                turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
            }
        }
        yield break;
    }

}
