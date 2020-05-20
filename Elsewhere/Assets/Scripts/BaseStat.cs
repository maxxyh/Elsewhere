using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat
{
    // public List<StatBonus> StatBonuses { get; set; }
    // public int BaseValue { get; set; }
    // public string StatName { get; set; }
    // public string StatDescription { get; set; }
    // public int FinalValue { get; set; }

    // public BaseStat(int BaseValue, string StatName, string StatDescription) 
    // {
    //     this.StatBonuses = new List<StatBonus>();
    //     this.BaseValue = BaseValue;
    //     this.StatName = StatName;
    //     this.StatDescription = StatDescription;
    // }

    // public void AddStatBonus(StatBonus statBonus)
    // {
    //     this.StatBonuses.Add(statBonus);
    // }

    // public void RemoveStatBonus(StatBonus statBonus)
    // {
    //     this.StatBonuses.Remove(statBonus);
    // }

    // public int GetFinalStatValue() {
    //     this.StatBonuses.ForEach (x => this.FinalValue += x.BonusValue);
    //     this.FinalValue += this.BaseValue;
    //     return FinalValue;
    // }

    public int baseValue;
    private List<int> modifiers = new List<int>();
    public int GetValue() 
    {
        int finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }

    public void AddModifier(int modifier) 
    {
        if (modifier != 0) {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier(int modifier) 
    {
        if (modifier != 0) {
            modifiers.Remove(modifier);
        }
    }
}
