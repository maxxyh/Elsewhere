using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuffItemEffect : UsableItemEffect
{
	public int AgilityBuff;
	public float Duration;

	public override void ExecuteEffect(UsableItem parentItem, UnitInventoryManager character)
	{
		StatModifier statModifier = new StatModifier(AgilityBuff, StatModType.Flat, parentItem);
		// character.Agility.AddModifier(statModifier);
		character.UpdateStatValues();
		character.StartCoroutine(RemoveBuff(character, statModifier, Duration));
	}

	public override string GetDescription()
	{
		return "Grants " + AgilityBuff + " Agility for " + Duration + " seconds.";
	}

	private static IEnumerator RemoveBuff(UnitInventoryManager character, StatModifier statModifier, float duration)
	{
		yield return new WaitForSeconds(duration);
		// character.Agility.RemoveModifier(statModifier);
		character.UpdateStatValues();
	}
}
