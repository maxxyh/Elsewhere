using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "consumableItem", menuName = "ScriptableObjects/Consumable Item")]
public class ConsumableItem : Item, IUseable
{
    [SerializeField] public List<ItemStatModifier> modifiers;

    public void Use(Dictionary<StatString, UnitStat> unitStats)
    {
        if (numUses <= 0)
        {
            Debug.LogError("Tried to use item when no longer any left");
            return;
        }
        foreach(ItemStatModifier entry in modifiers)
        {
            unitStats[entry.targetStat].AddModifier(new StatModifier(entry.value, entry.statModType));
        }
        numUses--;
        if (numUses == 0)
        {
            inventory.RemoveItem(this);
        }
    }
}

[System.Serializable]
public class ItemStatModifier
{
    [SerializeField] public StatString targetStat;
    [SerializeField] public float value;
    [SerializeField] public StatModType statModType;
}
  