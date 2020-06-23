using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level InitialUnitInfo", menuName = "ScriptableObjects/New InitialUnitInfo")]
public class IntialUnitInfo : ScriptableObject
{
    [SerializeField]
    public UnitInfoSO[] playerList;
    [SerializeField]
    public UnitInfoSO[] enemyList;
}

[System.Serializable]
public class UnitInfoSO
{
    [SerializeField]
    public string unitID;
    [SerializeField]
    public GameObject UnitPrefab;
    [SerializeField]
    public Vector3Int UnitPositions;
}




