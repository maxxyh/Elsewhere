using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public List<BaseStat> stats = new List<BaseStat>();

    void Start() 
    {
        Debug.Log("Helloooo");
        //can use a class system to determine the values of base stats
        stats.Add(new BaseStat(4, "Attack damage", "Physical damage."));
        
        //Add 5 to the attack damage;
        stats[0].AddStatBonus(new StatBonus(5));
        
        Debug.Log(stats[0].GetFinalStatValue());
    }
}
