using System.Collections;
using UnityEngine;

public class StatBonus 
{
    public int BonusValue { get; set; }

    public StatBonus(int bonusValue) 
    {
        this.BonusValue = bonusValue;
        Debug.Log("A new stat bonus!");
    }
}
