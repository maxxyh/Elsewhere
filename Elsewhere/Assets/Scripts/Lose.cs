using UnityEngine;
using System.Collections;

public class Lose : State
{
    public Lose(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        

        // show the lose screen
        // Lose panel will show 2 buttons: retry, return to main Screen

        yield break;
    }
}
