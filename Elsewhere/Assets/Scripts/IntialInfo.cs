using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level InitialUnitInfo", menuName = "ScriptableObjects/New InitialUnitInfo")]
public class IntialInfo : ScriptableObject
{
    [SerializeField]
    public GameObjectInfo[] playerList;
    [SerializeField]
    public GameObjectInfo[] enemyList;
    [SerializeField]
    public GameObjectInfo[] crystalList;
}

[System.Serializable]
public class GameObjectInfo
{
    [SerializeField]
    public string unitID;
    [SerializeField]
    public GameObject UnitPrefab;
    [SerializeField]
    public Vector3Int UnitPositions;
}




