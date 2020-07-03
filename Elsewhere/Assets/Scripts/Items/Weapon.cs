using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : Item, IEquippable
{
    [SerializeField] public int might;
    [SerializeField] public int hitRate;
    [SerializeField] public int critRate;
    [SerializeField] public int range;
    public bool broken = false;
    public bool equipped = false;
    private Dictionary<StatString, int> brokenWeaponStats = new Dictionary<StatString, int>()
    {
        {StatString.MIGHT, 0 },
        {StatString.HIT_RATE, 30},
        {StatString.CRIT_RATE, 0},
        {StatString.ATTACK_RANGE, 1}
    };
    private Dictionary<StatString, int> weaponStats = new Dictionary<StatString, int>()
    {
        {StatString.MIGHT, 0 },
        {StatString.HIT_RATE, 30},
        {StatString.CRIT_RATE, 0},
        {StatString.ATTACK_RANGE, 1}
    };


    public void Equip()
    {
        equipped = true;
    }

    public void UnEquip()
    {
        equipped = false;
    }

    public Dictionary<StatString, int> GetEquipBonus()
    {
        if (!broken)
        {
            weaponStats[StatString.MIGHT] = might;
            weaponStats[StatString.ATTACK_RANGE] = range;
            weaponStats[StatString.HIT_RATE] = hitRate;
            weaponStats[StatString.CRIT_RATE] = critRate;
            return weaponStats;
        }
        else
        {
            return brokenWeaponStats;
        }
    }

    public void UseWeapon()
    {
        if (!broken)
        {
            numUses--;
            if (numUses == 0)
            {
                broken = true;
            }
        }
    }
}


