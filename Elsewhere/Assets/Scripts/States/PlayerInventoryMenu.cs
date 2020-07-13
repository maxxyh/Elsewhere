using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryMenu : State
{
    public PlayerInventoryMenu(TurnScheduler turnScheduler) : base(turnScheduler)
    {

    }
    public override IEnumerator Execute()
    {
        turnScheduler.playerInventoryPanel.SetActive(true);

        yield break;
    }

    public override IEnumerator UsedUsableItem()
    {
        // TODO add waituntil used item animation
        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
        yield break;
    }

    public override IEnumerator ReturnPreviousMenu()
    {
        turnScheduler.playerInventoryPanel.SetActive(false);
        turnScheduler.SetState(new PlayerUnitSelected(turnScheduler));
        yield break;
    }
}
