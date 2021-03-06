﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbility : EnemyState
{
    private Ability _ability;
    public EnemyAbility(TurnScheduler turnScheduler) : base(turnScheduler)
    {
        _ability = currUnit.chosenAbility;
    }

    public override IEnumerator Execute()
    {
        List<Unit> targetUnits = turnScheduler.currUnit.abilityTargetUnits;
        map.RemoveAttackableTiles();

        turnScheduler.StartCoroutine(turnScheduler.AbilityAnimation(currUnit));
        yield return new WaitUntil(() => currUnit.anim.GetBool("isAbility") == false);

        yield return turnScheduler.StartCoroutine(_ability.Execute(turnScheduler.currUnit, targetUnits));    
        
        
        int exp = 0;
        foreach (Unit target in targetUnits)
        {
            bool killed = false;
            if (target.isDead())
            {
                yield return turnScheduler.StartCoroutine(turnScheduler.RemoveUnit(target));
                killed = true;
            }
            else
            {
                target.UpdateUI();
            }
            // add exp
            exp += Level.CalculateExp(target.level, currUnit.level, killed, _ability.targetsSameTeam);
        }
        currUnit.level.AddExp(exp);
        currUnit.UpdateUI();

        turnScheduler.currUnit.chosenAbility = null;
        targetUnits.Clear();
        yield return new WaitForSeconds(1f);

        turnScheduler.SetState(new EnemyEndTurn(turnScheduler));

    }
}