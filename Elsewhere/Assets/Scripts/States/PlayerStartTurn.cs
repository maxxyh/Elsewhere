/** To be treated as both Start Turn and Movement Phase
 */


using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerStartTurn : State
{
    public PlayerStartTurn(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        Debug.Log("STARTING PLAYER TURN");
        
        // this added layer is to support cancel functionality
        if (currUnit.CurrState == UnitState.ENDTURN)
        { 
            currUnit.StartTurn();
        } 
        else
        {
            currUnit.CurrState = UnitState.IDLING;
        }
        turnScheduler.playerActionPanel.SetActive(true);
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);

        // menu will be available for viewing...

        yield break;
    }

    public override IEnumerator Targeting(ActionType actType)
    {
        if (currUnit.CurrState == UnitState.IDLING) 
        {
            if (actType == ActionType.ATTACK)
            {
                map.RemoveSelectableTiles(currUnit.currentTile, false);
                turnScheduler.SetState(new PlayerAttackTargeting(turnScheduler));
            }
        }
        yield break;
    }

    public override IEnumerator OpenMenu(MenuType menuType)
    {
        if (menuType == MenuType.ABILITY)
        {
            map.RemoveSelectableTiles(turnScheduler.currUnit.currentTile, false);
            turnScheduler.SetState(new PlayerAbilityMenu(turnScheduler));
        }
        yield break;
    }

    // used to change active player
    public override IEnumerator CheckTargeting(Tile tile)
    {
        Unit switchUnit = null;
        foreach(Unit unit in turnScheduler.currTeamQueue)
        {
            if (unit.currentTile == tile)
            {
                switchUnit = unit;
                break;
            }
        }

        if (switchUnit != null)
        {
            currUnit.ReturnToStartTile();
            Debug.Log("CURRENT TILE OCCUPIED " + turnScheduler.currUnit.currentTile.occupied);
            turnScheduler.currTeamQueue.AddLast(turnScheduler.currUnit);
            turnScheduler.currTeamQueue.Remove(switchUnit);
            turnScheduler.currTeamQueue.AddFirst(switchUnit);
            turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
        }    
        yield break;
    }

}
