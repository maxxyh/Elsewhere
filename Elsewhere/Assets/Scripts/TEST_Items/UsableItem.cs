using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Usable Item")]
public class UsableItem : Item
{
    public bool IsConsumable;
    // TEST_InventoryManager = Unit for now
    public List<UsableItemEffect> effects;

    public override Item GetCopy()
    {
        return Instantiate(this);
    }

    public virtual void Use(InBattleUnitInventoryManager unit)
    {
        foreach (UsableItemEffect effect in effects)
        {
            effect.ExecuteEffect(this, unit);
        }
    }

    public override string GetItemType()
    {
        return IsConsumable ? "Consumable" : "Usable";
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        foreach (UsableItemEffect effect in effects)
        {
            sb.AppendLine(effect.GetDescription());
        }
        return sb.ToString();
    }
}
