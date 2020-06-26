using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    [Header("Text")]
    public GameObject unitName;
    public GameObject unitClass;
    public Text unitHP;
    public Text unitMana;
    public Text unitPhysicalDamage;
    public Text unitMagicDamage;
    public Text unitArmor;
    public Text unitMagicRes;
    public Text unitMovementRange;
    public Text unitAttackRange;

    [Header("Button")]
    public Button unitHPButton;
    public Button unitManaButton;
    public Button unitPhysicalDamageButton;
    public Button unitMagicDamageButton;
    public Button unitArmorButton;
    public Button unitMagicResButton;
    public Button unitMovementRangeButton;
    public Button unitAttackRangeButton;

    public void UpdateStatsUI(Dictionary<StatString, UnitStat> stats)
    {
        unitHP.text = stats[StatString.HP].Value.ToString() + "/" + stats[StatString.HP].baseValue.ToString();
        unitMana.text = stats[StatString.MANA].Value.ToString() + "/" + stats[StatString.MANA].baseValue.ToString(); ;
        unitPhysicalDamage.text = stats[StatString.PHYSICAL_DAMAGE].Value.ToString() + DisplayBuff(stats[StatString.PHYSICAL_DAMAGE].GetPercentageModifierAmount());
        unitMagicDamage.text = stats[StatString.MAGIC_DAMAGE].Value.ToString() + DisplayBuff(stats[StatString.MAGIC_DAMAGE].GetPercentageModifierAmount());
        unitArmor.text = stats[StatString.ARMOR].Value.ToString() + DisplayBuff(stats[StatString.ARMOR].GetPercentageModifierAmount());
        unitMagicRes.text = stats[StatString.MAGIC_RES].Value.ToString() + DisplayBuff(stats[StatString.MAGIC_RES].GetPercentageModifierAmount());
        unitMovementRange.text = stats[StatString.MOVEMENT_RANGE].Value.ToString();
        unitAttackRange.text = stats[StatString.ATTACK_RANGE].Value.ToString();
    }


    private static string DisplayBuff(float amount)
    {
        // round to 1 d.p.
        amount = Mathf.Round(amount * 10) / 10;

        if (amount == 0)
        {
            return "";
        }
        else if (amount > 0)
        {
            return $"(+{amount})";
        }
        else
        {
            return $"(-{Math.Abs(amount)})";
        }
    }

}
