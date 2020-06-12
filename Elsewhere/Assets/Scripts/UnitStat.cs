using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

[Serializable]
public class UnitStat 
{
    public float baseValue;
    protected readonly List<StatModifier> statModifiers;
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
    
    public UnitStat(float baseValue, bool hasLimit=false) : this()
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

        for (int i = 0; i < statModifiers.Count; i++) 
        {
            StatModifier mod = statModifiers[i];
            if (mod.type == StatModType.Flat) 
            {
                finalValue += mod.value;
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
                    sumPercentAdd = 0;
                } 
            }
        }
        if (hasLimit)
        {
            return (float)Math.Min(baseValue, Math.Round(finalValue, 4));
        } else
        {
            return (float)Math.Round(finalValue, 4);
        }
        
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
}