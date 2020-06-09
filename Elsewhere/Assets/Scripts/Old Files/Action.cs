using UnityEngine;
using System;

public abstract class Action : IComparable<Action>
{
    //[SerializeField] public GameObject mapObject;
    protected Map map;
    public Unit currUnit;
    public int actionID;
    public Action(int actionID, Unit currUnit)
    {
        this.actionID = actionID;
        this.currUnit = currUnit;
        this.map = GameObject.FindObjectOfType<Map>();
    }

    public abstract Action GenerateNextAction();

    public int CompareTo(Action other) 
    {
        if (this.currUnit.unitID != other.currUnit.unitID)
        {
            return this.currUnit.unitID - other.currUnit.unitID;
        }
        else if (this.actionID != other.actionID)
        {
            return this.actionID - other.actionID;
        }
        else
        {
            return 0;
        }
    }
}
