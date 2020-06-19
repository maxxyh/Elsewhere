using System;

public class BattleManager
{
    public static void Battle(Unit attacker, Unit recipient) 
    {
        TurnScheduler turnScheduler = GameAssets.MyInstance.turnScheduler;

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

    public static int CalculateBaseDamage(IUnit attacker, IUnit recipient)
    {
        return Math.Max(1, (int) Math.Ceiling(attacker.stats[StatString.PHYSICAL_DAMAGE].Value - recipient.stats[StatString.ARMOR].Value));
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
