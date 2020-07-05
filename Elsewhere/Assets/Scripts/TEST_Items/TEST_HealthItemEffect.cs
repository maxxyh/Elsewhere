using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Usable Item Effects / Heal")]
public class TEST_HealthItemEffect : TEST_UsableItemEffect
{
    public int healAmount;
    public override void ExecuteEffect(TEST_UsableItem parentItem, TEST_Unit unit)
    {
        unit.health += healAmount;
    }

    public override string GetDescription()
    {
        return "Heal for +" + healAmount + " HP";
    }
}
