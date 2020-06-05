using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static void Battle(Unit attacker, Unit recipient) 
    {
        TurnScheduler turnScheduler = FindObjectOfType<TurnScheduler>();

        if (!recipient.isDead()) 
        {
            attacker.BasicAttack();
        } 
        
        if (recipient.isDead())
        {
            turnScheduler.StartCoroutine(turnScheduler.RemoveUnit(recipient));
        } 
        else
        {
            recipient.UpdateUI();
        }
    }

}
