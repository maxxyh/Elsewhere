using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : Action
{
    public MoveAction(int actionID, Unit currUnit) : base(actionID, currUnit) { }
    public override Action GenerateNextAction() 
    {
        /*
        while (true)
        {
            if (!currUnit.takingTurn)
            {
                return new EndAction(actionID + 1, currUnit);
            }
            if (currUnit.attackingPhase)
            {
                return new AttackAction(actionID + 1, currUnit);
            }
        }
        */
        //currUnit.WaitForAttackOrEndTurn();
        if (!currUnit.takingTurn)
        {
            return new EndAction(actionID + 1, currUnit);
        }
        if (currUnit.attackingPhase)
        {
            return new AttackAction(actionID + 1, currUnit);
        } 
        else
        {
            Debug.Log("ERROR: Ended turn before endAction or attackAction detected.");
            return null;
        }
    }
}
