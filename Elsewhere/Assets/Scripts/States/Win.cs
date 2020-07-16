using UnityEngine;
using System.Collections;
using System.Linq;

public class Win : State
{
    public Win(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(1);
        turnScheduler.victoryUI.SetActive(true);
        turnScheduler.OnSaveGame?.Invoke(turnScheduler.deadPlayers.Concat(turnScheduler.players).ToList());
        turnScheduler.OnWinUpdateLevelData?.Invoke();

        // show the win screen
        // win panel will appear showing loots and xp gains from items with 2 Buttons: Continue Story, Return To Main Screen

        yield break;
    }
}
