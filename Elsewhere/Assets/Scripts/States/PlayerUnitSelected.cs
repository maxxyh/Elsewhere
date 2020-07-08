/** To be treated as both Start Turn and Movement Phase
 */


using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerUnitSelected : State
{
    public PlayerUnitSelected(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        // This supports cancel functionality - if came from endTurn then it's normal, otherwise it came from another state
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
        if (currUnit.CurrState == UnitState.IDLING)
        {
            if (menuType == MenuType.ABILITY)
            {
                map.RemoveSelectableTiles(turnScheduler.currUnit.currentTile, false);
                turnScheduler.abilitiesPanel.SetActive(true);
                turnScheduler.playerActionPanel.SetActive(false);
                turnScheduler.SetState(new PlayerAbilityMenu(turnScheduler));
            }
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
            map.RemoveSelectableTiles(currUnit.currentTile);
            currUnit.MakeInactive();
            turnScheduler.currTeamQueue.AddLast(turnScheduler.currUnit);
            turnScheduler.currTeamQueue.Remove(switchUnit);
            turnScheduler.currUnit = switchUnit;
            turnScheduler.SetState(new PlayerUnitSelected(turnScheduler));
        }    
        yield break;
    }


    public override IEnumerator AllCrystalsCollectedWin()
    {
        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
        yield break;
    }

    public override IEnumerator Capture()
    {
        Unit.OnCaptureCrystal(currUnit);
        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
        yield break;
    }

}
