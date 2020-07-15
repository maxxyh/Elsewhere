using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class StatBuffItemEffect : UsableItemEffect
{
	public int statBuff;
	public StatModType statModType;
	public StatString targetStat;
	public float duration;

	public override void ExecuteEffect(UsableItem parentItem, Unit unit)
	{
		StatModifier statModifier;
		if (statModType == StatModType.Flat)
		{
			statModifier = new StatModifier(statBuff, statModType, parentItem);
		}
		else
		{
			statModifier = new StatModifier(((float)statBuff) /100f, statModType, parentItem);
		}
		unit.stats[targetStat].AddModifier(statModifier);
		// character.Agility.AddModifier(statModifier);
		// character.StartCoroutine(RemoveBuff(character, statModifier, duration));
	}

	public override string GetDescription()
	{
		if (statModType == StatModType.Flat)
		{
			return $"Grants + {statBuff} {ConvertToString(targetStat)} till mission end.";
		}
		else if (statModType == StatModType.PercentAdd )
		{
			return $"Grants + {statBuff}% {ConvertToString(targetStat)} till mission end (additive).";
		}
		else
		{
			return $"Grants + {statBuff}% {ConvertToString(targetStat)} till mission end (compound).";
		}
	}

	private static string ConvertToString(StatString statString)
	{
		CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
		TextInfo textInfo = cultureInfo.TextInfo;
		return textInfo.ToTitleCase(statString.ToString().Replace('_', ' '));
	}

	// for traditional rpgs where buffs last for a certain amount of time
	/*private static IEnumerator RemoveBuff(InBattleUnitInventoryManager character, StatModifier statModifier, float duration)
	{
		yield return new WaitForSeconds(duration);
		// character.Agility.RemoveModifier(statModifier);
		character.UpdateStatValues();
	}*/
}
