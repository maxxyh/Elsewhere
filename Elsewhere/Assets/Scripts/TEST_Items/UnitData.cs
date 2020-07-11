using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData 
{
    public string unitID;
    public Sprite unitSprite;
    public Dictionary<StatString, UnitStat> stats;
    public List<Item> unitItems;
    
    public UnitData(string unitID, Dictionary<StatString, UnitStat> stats, List<Item> unitItems)
    {
        this.unitID = unitID;
        this.unitSprite = Resources.Load<Sprite>("Sprites/" + unitID);
        this.stats = stats;
        this.unitItems = unitItems;
    }
}
