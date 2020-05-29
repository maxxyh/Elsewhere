using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAction : Action
{
     public EndAction(int eventID, Unit currUnit) : base(eventID, currUnit) {
        
    }
    public override Action GenerateNextAction() 
    {
        map.RemoveSelectedTiles(currUnit.currentTile);
        return null;
    }
}
