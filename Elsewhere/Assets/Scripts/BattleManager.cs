using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static void Battle(Unit attacker, Unit recipient) 
    {
        TurnScheduler turnScheduler = FindObjectOfType<TurnScheduler>();

        if (!recipient.isDead()) 
        {
            // must take into account whether the main attack is magic or physical
            int attackDamage = CalculateBaseDamage(attacker, recipient); 
            recipient.TakeDamage(attackDamage);
            DamagePopUp.Create(recipient.transform.position, string.Format("- {0} HP", (int)attackDamage), PopupType.DAMAGE);

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

    public static int CalculateBaseDamage(Unit attacker, Unit recipient)
    {
        return Math.Max(1, (int) Math.Ceiling(attacker.stats["attackDamage"].Value - recipient.stats["armor"].Value));
    }

    public static int CalculatePhysicalDamage(float attackDamage, Unit recipient)
    {
        return Math.Max(1, (int)Math.Ceiling(attackDamage - recipient.stats["armor"].Value));
    }

    public static int CalculateMagicDamage(float attackDamage, Unit recipient)
    {
        return Math.Max(1, (int)Math.Ceiling(attackDamage - recipient.stats["magicRes"].Value));
    }
}
