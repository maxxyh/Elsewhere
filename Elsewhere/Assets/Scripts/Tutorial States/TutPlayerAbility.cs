using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutPlayerAbility : PlayerAbility
{
    public TutPlayerAbility(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        turnScheduler.abilityExecutingDialogue.SetActive(false);
        
        // TODO change to list style in Unit.cs
        List<Unit> targetUnits = turnScheduler.currUnit.abilityTargetUnits;

        map.RemoveAttackableTiles();

        yield return turnScheduler.StartCoroutine(turnScheduler.AbilityAnimation(currUnit));
        yield return turnScheduler.StartCoroutine(turnScheduler.currUnit.chosenAbility.Execute(turnScheduler.currUnit, targetUnits));

        foreach (Unit unit in targetUnits)
        {
            if (unit.isDead())
            {
                yield return turnScheduler.StartCoroutine(turnScheduler.RemoveUnit(unit));
            }
            else
            {
                unit.UpdateUI();
            }
        }

        targetUnits.Clear();

        turnScheduler.SetState(new TutPlayerEndTurn(turnScheduler));

        yield break;
    }
}
