using System;
using UnityEngine;

namespace LevelSelection
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "ScriptableObjects/LevelDatabase", order = 0)]
    public class LevelDatabase : ScriptableObject
    {
        public LevelDatabaseEntry[] levelDatabaseEntries;
    }

    [Serializable]
    public class LevelDatabaseEntry
    {
        public string levelId;
        public string sceneName;
        public InitialUnitInfo initialUnitInfo;

        public int GetNumPlayers()
        {
            return initialUnitInfo.playerList.Length;
        }
    }
}