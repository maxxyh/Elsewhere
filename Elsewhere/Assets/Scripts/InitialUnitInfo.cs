using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level Initial Unit Info", menuName = "ScriptableObjects/New Initial Unit Info")]
public class InitialUnitInfo : ScriptableObject
{
    [SerializeField]
    public UnitInfo[] playerList;
    [SerializeField]
    public UnitInfo[] enemyList;
    [SerializeField]
    public CrystalInfo[] crystalList;
}

[System.Serializable]
public class UnitInfo
{
    [SerializeField]
    public string unitID;
    [SerializeField]
    public GameObject UnitPrefab;
    [SerializeField]
    public Vector3Int UnitPositions;
}

[System.Serializable]
public class CrystalInfo
{
    [SerializeField]
    public GameObject UnitPrefab;
    [SerializeField]
    public Vector3Int UnitPositions;
}



