using UnityEngine;
using System.Collections;
using TMPro;

public class TutPlayerUnitSelected : PlayerUnitSelected
{
    public TutPlayerUnitSelected(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        if (turnScheduler.TutTurn == 1)
        {
            turnScheduler.moveDiaglogue.SetActive(true);
        }
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
        if (currUnit.characterName != "Julius" && currUnit.CurrState == UnitState.IDLING)
        {
            if (actType == ActionType.ATTACK)
            {
                map.RemoveSelectableTiles(currUnit.currentTile, false);
                turnScheduler.SetState(new TutPlayerAttackTargeting(turnScheduler));
            }
        }
        yield break;
    }

    public override IEnumerator OpenMenu(MenuType menuType)
    {
        
        if (currUnit.characterName == "Julius" && currUnit.CurrState == UnitState.IDLING)
        {
            if (menuType == MenuType.ABILITY)
            {
                map.RemoveSelectableTiles(turnScheduler.currUnit.currentTile, false);
                turnScheduler.abilitiesPanel.SetActive(true);
                turnScheduler.playerActionPanel.SetActive(false);
                turnScheduler.SetState(new TutPlayerAbilityMenu(turnScheduler));
            }
        }
        yield break;
    }
    public override IEnumerator CheckTargeting(Tile tile)
    {
        Unit switchUnit = null;
        foreach (Unit unit in turnScheduler.currTeamQueue)
        {
            if (currUnit.characterName == "Esmeralda")
            {
                if (unit.currentTile == tile && unit.characterName == "Esmeralda")
                {
                    Debug.Log("In targeting of unit selected");
                    switchUnit = unit;
                    break;
                }
            } 
            else if (currUnit.characterName == "Julius")
            {
                if (unit.currentTile == tile && unit.characterName == "Julius")
                {
                    switchUnit = unit;
                    break;
                }
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
            turnScheduler.SetState(new TutPlayerUnitSelected(turnScheduler));
        }
        yield break;
    }
    public override IEnumerator EndTurn()
    {
        yield break;
    }
}
