using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDatabase", menuName = "ScriptableObjects/Unit Database", order = 0)]
public class UnitDatabase : ScriptableObject
{
    [SerializeField] public List<UnitDataEntry> UnitDataEntries;
}

[System.Serializable]
public class UnitDataEntry
{ 
    [SerializeField] public string unitName;
    [SerializeField] public GameObject unitPrefab;
}