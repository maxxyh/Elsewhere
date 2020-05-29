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
        currUnit.isAttacking = false;
        currUnit.attackingPhase = false;
        return null;
    }
}
