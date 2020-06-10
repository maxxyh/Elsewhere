using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAction : Action
{   
    public StartAction(Unit currUnit) : base(1, currUnit) {

    }

    public override Action GenerateNextAction() 
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].Value);
        
        return new MoveAction(actionID + 1, currUnit);
    }
}
