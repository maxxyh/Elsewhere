using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items / Usable Item")]
public class TEST_UsableItem : TEST_Item
{
    public bool IsConsumable;
    // TEST_InventoryManager = Unit for now
    public List<TEST_UsableItemEffect> effects;
    public virtual void Use(TEST_Unit unit)
    {
        foreach (TEST_UsableItemEffect effect in effects)
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
        foreach (TEST_UsableItemEffect effect in effects)
        {
            sb.AppendLine(effect.GetDescription());
        }
        return sb.ToString();
    }
}
