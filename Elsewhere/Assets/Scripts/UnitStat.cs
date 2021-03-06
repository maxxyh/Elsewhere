﻿using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class UnitStat 
{
    public float baseValue;
    protected List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers_readonly;
    public virtual float Value { 
        get {
            if (isDirty || baseValue != lastBaseValue) {
                lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            } 
            return _value;
        }
    }

    protected float lastBaseValue = float.MinValue;
   
    //indicate if we need to recalculate the value or not
    protected bool isDirty = true;

    // indicate if can exceed the base value
    protected bool hasLimit;
   
    //holds most recent calculation that we did
    protected float _value;

    private UnitStat() {
        this.statModifiers = new List<StatModifier>();
        this.StatModifiers_readonly = statModifiers.AsReadOnly();
    }
    
    public UnitStat(float baseValue, bool hasLimit = false) : this()
    {
        this.baseValue = baseValue;
        this.hasLimit = hasLimit;
    }

    public virtual void AddModifier(StatModifier mod) 
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }

    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b) {
        if (a.order < b.order) 
        {
            return -1;
        } 
        else if (a.order > b.order)
        {
            return 1;
        } 
        else 
        {
            return 0;
        }
    }

    public virtual bool RemoveModifier(StatModifier mod) 
    {
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual float CalculateFinalValue() 
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;

        // Debug.Log("starting value = " + finalValue);

        for (int i = 0; i < statModifiers.Count; i++) 
        {
            StatModifier mod = statModifiers[i];
            if (mod.type == StatModType.Flat) 
            {
                if (finalValue + mod.value > baseValue && hasLimit)
                {
                    finalValue = baseValue;
                } 
                else if (finalValue + mod.value < 0)
                {
                    finalValue = 0;
                }
                else
                {
                    finalValue += mod.value;
                }
            } 
            else if (mod.type == StatModType.PercentMult) 
            {
                finalValue *= 1 + mod.value;  
            }
            else if (mod.type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.value;
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatModType.PercentAdd) 
                {
                    finalValue *= 1 + sumPercentAdd;
                    // Debug.Log("current value = " + finalValue);
                    sumPercentAdd = 0;
                } 
            }
        }
        // adding a round to 1 d.p.
        return Mathf.Round(finalValue * 10) / 10;
    }

    public virtual bool RemoveAllModifiersFromSource(object source) 
    {
        bool didRemove = false;
        for (int i = statModifiers.Count - 1; i >= 0; i--) 
        {
            if (statModifiers[i].source == source) 
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    public void DecrementDuration()
    {
        List<StatModifier> toRemove = new List<StatModifier>();
        foreach(StatModifier statModifier in statModifiers)
        {
            if (statModifier.type == StatModType.PercentAdd || statModifier.type == StatModType.PercentMult)
            {
                if (statModifier.duration > 1)
                {
                    statModifier.duration--;
                }
                else if (statModifier.duration == 1)
                {
                    toRemove.Add(statModifier);
                }
            }
        }
        
        foreach(StatModifier sm in toRemove)
        {
            RemoveModifier(sm);
        }
    }

    public float GetPercentageModifierAmount()
    {
        float percentMult = 1;
        float sumPercentAdd = 1;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            if (mod.type == StatModType.PercentMult)
            {
                percentMult *= mod.value;
            }
            else if (mod.type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.value;
            }
        }

        float result = sumPercentAdd * percentMult;

        float finalValue = CalculateFinalValue();

        return finalValue - finalValue / result;
    }

    // used when levelling up
    public void IncreaseBaseValue(int gain)
    {
        baseValue += gain;
        _value += gain;
    }

    public void ReduceBaseValue(int decrement)
    {
        baseValue -= decrement;
        _value -= decrement;
    }
}

public enum StatString
{
    HP,
    MANA,
    PHYSICAL_DAMAGE,
    MAGIC_DAMAGE,
    ARMOR,
    MAGIC_RES,
    MOVEMENT_RANGE,
    
    ATTACK_RANGE,
    CRIT_RATE,
    HIT_RATE,
    MIGHT
}