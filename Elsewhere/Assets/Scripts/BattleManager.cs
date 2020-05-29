using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static void Battle(Unit attacker, Unit recipient) 
    {
        TurnScheduler turnScheduler = FindObjectOfType<TurnScheduler>();

        if (!recipient.isDead) 
        {
            attacker.BasicAttack();
        } 
        
        if (recipient.isDead)
        {
            turnScheduler.RemoveUnit(recipient);
        } 
    }

    public bool CheckIfDead(Unit unit) 
    {
        return (unit.currHP <= 0);
    }
}
