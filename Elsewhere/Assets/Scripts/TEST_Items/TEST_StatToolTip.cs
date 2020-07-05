using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Scripting;

public class TEST_StatToolTip : MonoBehaviour
{
    [SerializeField] Text statNameText;
    [SerializeField] Text statModifierText;

    private StringBuilder sb = new StringBuilder();

    public void ShowToolTip(UnitStat stat, string statName)
    {
        statNameText.text = GetStatTopText(stat, statName);
        statModifierText.text = GetStatModifiersText(stat);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    private string GetStatTopText(UnitStat stat, string statName)
    {
        sb.Length = 0;
        sb.Append(statName);
        sb.Append(" ");
        sb.Append(stat.Value);
        sb.Append(" (");
        sb.Append(stat.baseValue);

        if (stat.Value >= stat.baseValue)
        {
            sb.Append("+");
        }
        sb.Append(stat.Value - stat.baseValue);
        sb.Append(")");
        return sb.ToString();
    }

    private string GetStatModifiersText(UnitStat stat)
    {
        sb.Length = 0;
        foreach (StatModifier mod in stat.StatModifiers_readonly)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if (mod.value > 0)
            {
                sb.Append("+");
            }
            if (mod.type == StatModType.Flat)
            {
                sb.Append(mod.value);
            }
            else
            {
                sb.Append(mod.value * 100);
                sb.Append("%");
            }

            // "as" check if source is of type TEST_Equippable Item, if not just assign null
            TEST_Item item = mod.source as TEST_Item;

            if (item != null)
            {
                sb.Append(" ");
                sb.Append(item.itemName);
            }
            else
            {
                Debug.LogError("Modifier is not a TEST_Item item");
            }
        }
        return sb.ToString();
    }
}
