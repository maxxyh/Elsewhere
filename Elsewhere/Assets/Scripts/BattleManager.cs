using Boo.Lang;
using System;

public class BattleManager
{
    private static List<string> physicalClasses = new List<string>() { "Tank", "Swordsman", "Gunslinger" };
    private static List<string> magicClasses = new List<string>() { "Healer", "Sage"};
    public static void Battle(Unit attacker, Unit recipient) 
    {
        TurnScheduler turnScheduler = GameAssets.MyInstance.turnScheduler;

        if (!recipient.isDead()) 
        {
            // must take into account whether the main attack is magic or physical
            int attackDamage = CalculateBaseDamage(attacker, recipient); 
            recipient.TakeDamage(attackDamage);
            attacker.UseWeapon();
            DamagePopUp.Create(recipient.transform.position, string.Format("- {0} HP", (int)attackDamage), PopupType.DAMAGE);
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
