using Boo.Lang;
using System;
using System.Threading;

public class BattleManager
{
    private static List<string> physicalClasses = new List<string>() { "Tank", "Swordsman", "Gunslinger" };
    private static List<string> magicClasses = new List<string>() { "Healer", "Sage"};
    private static readonly int CritMultiplier = 2;
    public static void Battle(Unit attacker, Unit recipient) 
    {
        TurnScheduler turnScheduler = GameAssets.MyInstance.turnScheduler;
        
        bool hit = new Random().Next(1,100) <= attacker.stats[StatString.HIT_RATE].Value;
        bool crit = new Random().Next(1,100) <= attacker.stats[StatString.CRIT_RATE].Value;

        if (!recipient.isDead() ) 
        {
            if (hit)
            {
                // must take into account whether the main attack is magic or physical
                int attackDamage = CalculateBaseDamage(attacker, recipient);
                attackDamage = crit ? attackDamage * CritMultiplier : attackDamage; 
                recipient.TakeDamage(attackDamage);
                attacker.UseWeapon();
                DamagePopUp.Create(recipient.transform.position, string.Format("- {0} HP", (int) attackDamage),
                    PopupType.DAMAGE, crit);
            }
            else
            {
                DamagePopUp.Create(recipient.transform.position, "MISS!", PopupType.DAMAGE);
            }
        }

        bool killed = false;
        if (recipient.isDead())
        {
            turnScheduler.StartCoroutine(turnScheduler.RemoveUnit(recipient));
            killed = true;
        } 
        else
        {
            recipient.UpdateUI();
        }

        // add exp
        int exp = Level.CalculateExp(recipient.level, attacker.level, killed);
        attacker.level.AddExp(exp);
        attacker.UpdateUI();
    }

    public static int CalculateBaseDamage(IUnit attacker, IUnit recipient)
    {
        if (attacker.characterClass != null && magicClasses.Contains(attacker.characterClass))
        {
            return Math.Max(1, (int)Math.Ceiling(attacker.stats[StatString.MAGIC_DAMAGE].Value - recipient.stats[StatString.MAGIC_RES].Value));
        } 
        else
        {
            return Math.Max(1, (int)Math.Ceiling(attacker.stats[StatString.PHYSICAL_DAMAGE].Value - recipient.stats[StatString.ARMOR].Value));
        }
        
    }

    public static int CalculatePhysicalDamage(float attackDamage, IUnit recipient)
    {
        return Math.Max(1, (int)Math.Ceiling(attackDamage - recipient.stats[StatString.ARMOR].Value));
    }

    public static int CalculateMagicDamage(float attackDamage, IUnit recipient)
    {
        return Math.Max(1, (int)Math.Ceiling(attackDamage - recipient.stats[StatString.MAGIC_RES].Value));
    }
}
