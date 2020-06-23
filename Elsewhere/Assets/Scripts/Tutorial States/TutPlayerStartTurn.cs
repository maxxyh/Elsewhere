using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class TutPlayerStartTurn : PlayerStartTurn
{
    public TutPlayerStartTurn(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.startPlayerTurnDialogue.SetActive(true);
        yield break;
    }

    public override IEnumerator CheckTargeting(Tile tile)
    {
        yield return new WaitUntil(() => turnScheduler.startPlayerTurnDialogue.GetComponent<DialogueDisplay>().endConvo);
        Unit switchUnit = null;
        Debug.Log("Checkingtarget");
        Debug.Log("turn:" + turnScheduler.TutTurn);
        foreach (Unit unit in turnScheduler.currTeamQueue)
        {
            if (turnScheduler.TutTurn == 1)
            {
                if (unit.currentTile == tile && unit.characterName == "Esmeralda")
                {
                    Debug.Log("In targeting of unit selected");
                    switchUnit = unit;
                    break;
                }
            }
            else if (turnScheduler.TutTurn == 2)
            {
                if (unit.currentTile == tile && unit.characterName == "Julius")
                {
                    switchUnit = unit;
                    break;
                }
            }
        }

        if (switchUnit != null)
        {
            turnScheduler.currTeamQueue.Remove(switchUnit);
            turnScheduler.currUnit = switchUnit;
            turnScheduler.SetState(new TutPlayerUnitSelected(turnScheduler));
        }
    }
}
