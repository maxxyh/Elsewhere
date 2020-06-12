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
        // this added layer is to support cancel functionality
        if (currUnit.currState == UnitState.ENDTURN)
        {
            currUnit.StartTurn();
        } 
        else
        {
            currUnit.currState = UnitState.IDLING;
        }
        turnScheduler.playerActionPanel.SetActive(true);
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].Value);

        // menu will be available for viewing...

        yield break;
    }

    public override IEnumerator Targeting(ActionType actType)
    {
        if (currUnit.currState == UnitState.IDLING) 
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

}
