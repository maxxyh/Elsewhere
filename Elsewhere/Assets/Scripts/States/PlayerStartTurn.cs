using UnityEngine;
using System.Collections;

public class PlayerStartTurn : State
{
    public PlayerStartTurn(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        if (turnScheduler.openingDialogue.gameObject.activeSelf)
        {
            Debug.Log("in execute start player turn true");
            turnScheduler.WaitForDialogueEnd();
        }
        else
        {
            Debug.Log("in execute start player turn false");
        }
        yield break;
    }
    public override IEnumerator CheckTargeting(Tile tile)
    {
        Unit switchUnit = null;
        foreach (Unit unit in turnScheduler.currTeamQueue)
        {
            if (unit.currentTile == tile)
            {
                switchUnit = unit;
                break;
            }
        }

        if (switchUnit != null)
        {
            turnScheduler.currTeamQueue.Remove(switchUnit);
            turnScheduler.currUnit = switchUnit;
            turnScheduler.SetState(new PlayerUnitSelected(turnScheduler));
        }
        yield break;
    }

    public override IEnumerator DuringDialogue()
    {
        Debug.Log("in ienum start player turn");
        yield return new WaitUntil(() => turnScheduler.openingDialogue.endConvo);
        turnScheduler.openingDialogue.gameObject.SetActive(false);
    }
}
