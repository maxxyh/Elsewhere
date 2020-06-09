using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    private float attackRange;
    // private bool isSingleTarget;
    private float manaCost;
    private List<Unit> targets;

    public Ability(string abilityName, float attackRange, float manaCost, List<Unit> targets)
    {
        this.abilityName = abilityName;
        this.attackRange = attackRange;
        this.manaCost = manaCost;
        this.targets = targets;
    }

    public virtual IEnumerator Execute()
    {
        yield break;
    }
}
