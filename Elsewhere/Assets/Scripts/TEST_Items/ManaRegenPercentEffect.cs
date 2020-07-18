using UnityEngine;

[CreateAssetMenu(menuName = "Usable Item Effects / Mana Regen Percent")]
public class ManaRegenPercentEffect : UsableItemEffect
{
    public float regenAmount;
    public override void ExecuteEffect(UsableItem parentItem, Unit unit)
    {
        unit.stats[StatString.MANA].AddModifier(new StatModifier(regenAmount, StatModType.PercentAdd));
    }

    public override string GetDescription()
    {
        return "Regenerates +" + regenAmount * 100 + "% of current Mana";
    }
}