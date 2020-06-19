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

    public virtual IEnumerator Ability()
    {
        yield break;
    }

    public virtual IEnumerator Targeting(ActionType actType)
    {
        yield break;
    }

    public virtual IEnumerator CheckTargeting(Tile tile)
    {
        yield break;
    }

    public virtual IEnumerator Yes()
    {
        yield break;
    }

    public virtual IEnumerator No()
    {
        yield break;
    }

    public virtual IEnumerator Cancel()
    {
        yield break;
    }

    public virtual IEnumerator ReturnPreviousMenu()
    {
        yield break;
    }

    public virtual IEnumerator OpenMenu(MenuType menuType)
    {
        yield break;
    }

    public virtual IEnumerator DuringCutScene()
    {
        yield break;
    }

    public virtual IEnumerator EndTurn()
    {
        if (currUnit.currState == UnitState.IDLING)
        {
            if (turnScheduler.currTurn == Team.PLAYER)
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

public enum MenuType
{
    ABILITY
}