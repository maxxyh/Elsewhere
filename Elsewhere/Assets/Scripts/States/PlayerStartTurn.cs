using UnityEngine;
using System.Collections;

public class PlayerStartTurn : State
{
    public PlayerStartTurn(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }
    public override IEnumerator CheckTargeting(Tile tile)
    {
        Unit switchUnit = null;
        foreach (Unit unit in turnScheduler.currTeamQueue)
        {
            if (unit.currentTile == tile)
            {
                switchUnit = unit;
                break;
            }
        }

        if (switchUnit != null)
        {
            turnScheduler.currTeamQueue.Remove(switchUnit);
            turnScheduler.currUnit = switchUnit;
            turnScheduler.SetState(new PlayerUnitSelected(turnScheduler));
        }
        yield break;
    }
}
