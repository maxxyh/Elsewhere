using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public string abilityName;
    public float attackRange;
    public TargetingStyle targetingStyle;
    public bool targetsSameTeam;
    protected float manaCost;

    public Ability(string abilityName, float attackRange, float manaCost, bool targetsSameTeam, TargetingStyle targetingStyle)
    {
        this.abilityName = abilityName;
        this.attackRange = attackRange;
        this.manaCost = manaCost;
        this.targetsSameTeam = targetsSameTeam;
        this.targetingStyle = targetingStyle;
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

public enum TargetingStyle
{
    SINGLE,
    MULTI,
    RADIUS, 
    CONE, 
    SELF
}