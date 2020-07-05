using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TEST_UsableItemEffect : ScriptableObject
{
    public abstract void ExecuteEffect(TEST_UsableItem parentItem, TEST_Unit unit);

    public abstract string GetDescription();
}
