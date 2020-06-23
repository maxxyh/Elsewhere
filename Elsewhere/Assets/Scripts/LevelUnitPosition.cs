using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Level () UnitPosition", menuName = "ScriptableObjects/Level Unit Position")]
public class LevelUnitPosition : ScriptableObject
{
    [SerializeField]
    public Vector3Int[] PlayerPositions;
    [SerializeField]
    public Vector3Int[] EnemyPositions;
}  
