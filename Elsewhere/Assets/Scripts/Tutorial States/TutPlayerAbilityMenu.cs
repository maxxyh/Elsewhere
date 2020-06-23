using UnityEngine;
using System.Collections;

public class TutPlayerAbilityMenu : PlayerAbilityMenu
{
    public TutPlayerAbilityMenu(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.nextChooseDialogue.SetActive(false);
        turnScheduler.abilityClickDialogue.SetActive(true);
        turnScheduler.abilitiesPanel.SetActive(true);
        turnScheduler.currUnit.CurrState = UnitState.IDLING;
        yield break;
    }

    public override IEnumerator Targeting(ActionType actType)
    {
        yield return new WaitUntil(() => turnScheduler.abilityClickDialogue.GetComponent<DialogueDisplay>().endConvo);
        if (currUnit.CurrState == UnitState.IDLING)
        {
            if (actType == ActionType.ABILITY)
            {
                turnScheduler.SetState(new TutPlayerAbilityTargeting(turnScheduler));
            }
        }
        yield break;
    }

    public override IEnumerator ReturnPreviousMenu()
    {
        turnScheduler.abilitiesPanel.SetActive(false);
        turnScheduler.SetState(new TutPlayerUnitSelected(turnScheduler));
        yield break;
    }
}