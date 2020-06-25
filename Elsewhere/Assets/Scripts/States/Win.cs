using UnityEngine;
using System.Collections;

public class Win : State
{
    public Win(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.victoryUI.SetActive(true);

        // show the win screen
        // win panel will appear showing loots and xp gains from items with 2 Buttons: Continue Story, Return To Main Screen

        yield break;
    }
}
