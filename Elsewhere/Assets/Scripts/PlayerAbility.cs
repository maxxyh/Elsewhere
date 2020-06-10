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
        List<Unit> targetUnits = new List<Unit>();
        targetUnits.Add(turnScheduler.currUnit.abilityTargetUnit);

        map.RemoveAttackableTiles();    

        yield return turnScheduler.StartCoroutine(turnScheduler.currUnit.chosenAbility.Execute(targetUnits));
        
        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));

        yield break;
    }
}
