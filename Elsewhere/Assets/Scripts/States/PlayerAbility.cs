using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

public class PlayerAbility : State
{
    private Ability _ability;
    public PlayerAbility(TurnScheduler turnScheduler) : base(turnScheduler)
    {
        _ability = turnScheduler.currUnit.chosenAbility;
    }

    public override IEnumerator Execute()
    {
        List<Unit> targetUnits = turnScheduler.currUnit.abilityTargetUnits;
        map.RemoveAttackableTiles();

        turnScheduler.StartCoroutine(turnScheduler.AbilityAnimation(currUnit));
        yield return new WaitUntil(() => currUnit.anim.GetBool("isAbility") == false);

        if (_ability is AbilityWallShatter)
        {
            yield return turnScheduler.StartCoroutine(
                ((AbilityWallShatter) _ability).Execute(turnScheduler.currUnit, currUnit.abilityTargetTile, map));
        }
        else
        {
            yield return turnScheduler.StartCoroutine(_ability.Execute(turnScheduler.currUnit, targetUnits));    
        }

        if (_ability.abilityTypes.Contains(AbilityType.DAMAGE) ||
            _ability.abilityTypes.Contains(AbilityType.HEAL_SELF) ||
            _ability.abilityTypes.Contains(AbilityType.HEAL_TEAM))
        {
            currUnit.UseWeapon();
        }
        
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

        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));

        yield break;
    }
}
