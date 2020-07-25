using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerLose : State
{
    public PlayerLose(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(1);
        turnScheduler.gameOverUI.SetActive(true);
        turnScheduler.OnSaveGame?.Invoke(turnScheduler.deadPlayers.Concat(turnScheduler.players).ToList());

        // show the lose screen
        // Lose panel will show 2 buttons: retry, return to main Screen

        yield break;
    }
}
