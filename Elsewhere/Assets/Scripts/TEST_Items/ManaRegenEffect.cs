using UnityEngine;

[CreateAssetMenu(menuName = "Usable Item Effects / Mana Regen")]
public class ManaRegenEffect : UsableItemEffect
{
    public int regenAmount;
    public override void ExecuteEffect(UsableItem parentItem, Unit unit)
    {
        unit.stats[StatString.MANA].AddModifier(new StatModifier(regenAmount, StatModType.Flat));
    }

    public override string GetDescription()
    {
        return "Regenerates +" + regenAmount + " Mana";
    }
}