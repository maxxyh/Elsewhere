﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to be used pre-game when less data is required + a sprite is required
public class UnitData 
{
    public string unitID;
    public Sprite unitSprite;
    public Dictionary<StatString, UnitStat> stats;
    public List<string> unitAbilities;
    public List<Item> unitItems;
    public int unitLevel;
    public int unitExp;
    
    public UnitData(string unitID, Dictionary<StatString, UnitStat> stats, List<Item> unitItems, List<string> unitAbilities, int unitLevel, int unitExp)
    {
        this.unitID = unitID;
        this.unitSprite = Resources.Load<Sprite>("Sprites/" + unitID);
        this.stats = stats;
        this.unitItems = unitItems;
        this.unitAbilities = unitAbilities;
        this.unitLevel = unitLevel;
        this.unitExp = unitExp;
    }
}
