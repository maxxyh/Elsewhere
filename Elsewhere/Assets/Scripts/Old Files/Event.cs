/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Event : IComparable<Event>
{ 
    public EventType type;
    public Unit currUnit;

    public Event(Unit inUnit, EventType inEvent)
    {
        currUnit = inUnit;
        type = inEvent;
    }


    public int CompareTo(Event other)
    {
        if (this.currUnit.unitID != other.currUnit.unitID)
        {
            return this.currUnit.unitID - other.currUnit.unitID;
        }
        else if (this.type != other.type)
        {
            return this.type- other.type;
        }
        else
        {
            return 0;
        }
    }

}*/