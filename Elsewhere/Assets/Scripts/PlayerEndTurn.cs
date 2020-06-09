using UnityEngine;
using System.Collections;

public class PlayerEndTurn : State
{
    public PlayerEndTurn(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        yield break;
    }
}
