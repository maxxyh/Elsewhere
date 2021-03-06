﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Initial Unit Info", menuName = "ScriptableObjects/New Initial Unit Info")]
public class InitialUnitInfo : ScriptableObject
{
    [SerializeField] public UnitInfo[] playerList;
    [SerializeField]
    public UnitInfo[] enemyList;
    [SerializeField]
    public CrystalInfo[] crystalList;
    public string enemyStatsFilename;
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

    public void UpdateUnitIdAndPrefab(UnitDataEntry unitDataEntry)
    {
        unitID = unitDataEntry.unitName;
        UnitPrefab = unitDataEntry.unitPrefab;
    }
}

[System.Serializable]
public class CrystalInfo
{
    [SerializeField]
    public GameObject UnitPrefab;
    [SerializeField]
    public Vector3Int UnitPositions;
}



