using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public string abilityName;
    public float attackRange;
    public TargetingStyle targetingStyle;
    public AbilityType[] abilityTypes;
    public bool targetsSameTeam;
    public int multiAbilityRange;
    public int duration; 
    protected int manaCost;
    

    public Ability(string abilityName, float attackRange, int manaCost, bool targetsSameTeam, TargetingStyle targetingStyle, AbilityType[] abilityTypes, int multiAbilityRange = 0, int duration = -1)
    {
        this.abilityName = abilityName;
        this.attackRange = attackRange;
        this.manaCost = manaCost;
        this.targetsSameTeam = targetsSameTeam;
        this.targetingStyle = targetingStyle;
        this.abilityTypes = abilityTypes;
        this.multiAbilityRange = multiAbilityRange;
        this.duration = duration;
    }

    public virtual IEnumerator Execute(Unit initiator, List<Unit> targets)
    {
        yield break;
    }
    
    public int GetManaCost()
    {
        return manaCost;
    }

    public void UpdateStats(Unit initiator, List<Unit> targets)
    {
        Debug.Log("in ability base");
        foreach (Unit unit in targets)
        {
            unit.UpdateUI();
        }
        initiator.stats[StatString.MANA].AddModifier(new StatModifier(-manaCost, StatModType.Flat));
        initiator.UpdateUI();
    }
}

public enum TargetingStyle
{
    SINGLE,
    MULTI,
    RADIUS, 
    CONE, 
    SELF,
    SELFSINGLE
}

public enum AbilityType
{
    DAMAGE,
    HEAL_TEAM,
    HEAL_SELF,
    BUFF,
    DEBUFF
}