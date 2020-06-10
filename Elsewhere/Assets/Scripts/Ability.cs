using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public string abilityName;
    public float attackRange;
    public bool isSingleTarget;
    public bool targetsSameTeam;
    protected float manaCost;

    public Ability(string abilityName, float attackRange, float manaCost, bool targetsSameTeam, bool isSingleTarget)
    {
        this.abilityName = abilityName;
        this.attackRange = attackRange;
        this.manaCost = manaCost;
        this.targetsSameTeam = targetsSameTeam;
        this.isSingleTarget = isSingleTarget;
    }

    public virtual IEnumerator Execute(List<Unit> targets)
    {
        yield break;
    }
    
    public float GetManaCost()
    {
        return manaCost;
    }
}
