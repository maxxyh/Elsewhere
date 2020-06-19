using UnityEngine;
using System.Collections;

public class PlayerAbilityMenu : State
{
    public PlayerAbilityMenu(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.abilitiesPanel.SetActive(true);
        turnScheduler.currUnit.CurrState = UnitState.IDLING;
        yield break;
    }
    public override IEnumerator Targeting(ActionType actType)
    {
        if (currUnit.CurrState == UnitState.IDLING)
        {
            if (actType == ActionType.ABILITY)
            {
                turnScheduler.SetState(new PlayerAbilityTargeting(turnScheduler));
            }
        }
        yield break;
    }

    public override IEnumerator ReturnPreviousMenu()
    {
        turnScheduler.abilitiesPanel.SetActive(false);
        turnScheduler.SetState(new PlayerStartTurn(turnScheduler));
        yield break;
    }
}
