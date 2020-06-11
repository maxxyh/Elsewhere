﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealAbility : Ability
{
    public HealAbility() : base("Healing Waveeeeeeeeeeeee", 3, 4, true, true)
    {
    }

    public override IEnumerator Execute(List<Unit> targets)
    {
        foreach(Unit target in targets)
        {
            target.stats["HP"].AddModifier(new StatModifier(-5, StatModType.Flat));
            Debug.Log(target.stats["HP"].Value);
            target.UpdateUI();
        }

        yield break;
    }
}
