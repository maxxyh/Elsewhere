using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class PlayerAbility : State
{
    public PlayerAbility(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {

        // TODO change to list style in Unit.cs
        List<Unit> targetUnits = turnScheduler.currUnit.abilityTargetUnits;

        map.RemoveAttackableTiles();    

        yield return turnScheduler.StartCoroutine(turnScheduler.currUnit.chosenAbility.Execute(turnScheduler.currUnit, targetUnits));

        foreach (Unit unit in targetUnits)
        {
            if (unit.isDead())
            {
                turnScheduler.StartCoroutine(turnScheduler.RemoveUnit(unit));
            }
            else
            {
                unit.UpdateUI();
            }
        }

        targetUnits.Clear();

        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));

        yield break;
    }
}
