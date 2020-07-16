using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelSelection
{
    public class LevelSelectManager : MonoBehaviour
    {
        [SerializeField] private Transform levelButtonParent;
        [SerializeField] private LevelDatabase levelDatabase;
        
        public List<LevelSelectButton> levels;
        
        private void Start()
        {
            for (int i = 0; i < levelButtonParent.childCount; i++)
            {
                LevelSelectButton levelSelectButton = levelButtonParent.GetChild(i).GetComponent<LevelSelectButton>();
                levelSelectButton.onButtonClick += LoadLevel;
                if (!PlayerPrefs.HasKey(levelSelectButton.levelId))
                {
                    PlayerPrefs.SetInt(levelSelectButton.levelId, levelSelectButton.levelId != "Tutorial" ? 0 : 1);
                }

                if (PlayerPrefs.GetInt("Tutorial") == 0)
                {
                    PlayerPrefs.SetInt("Tutorial", 1);
                }
                levelSelectButton.ToggleSelectable(PlayerPrefs.GetInt(levelSelectButton.levelId));
                levels.Add(levelSelectButton);
            }
        }

        public void LoadLevel(string levelId)
        {
            bool valid = false; 
            for (int i = 0; i < levelDatabase.levelDatabaseEntries.Length; i++)
            {
                LevelDatabaseEntry temp = levelDatabase.levelDatabaseEntries[i];
                if (temp.levelId == levelId)
                {
                    StaticData.LevelInformation = temp;
                    valid = true;
                }
            }

            if (valid && SceneUtility.GetBuildIndexByScenePath(StaticData.LevelInformation.sceneName) >= 0)
            {
                if (levelId == "Tutorial")
                {
                    SceneManager.LoadScene("Tutorial");
                }
                else
                {
                    SceneManager.LoadScene("UnitSelection");
                }
            }
            else
            {
                Debug.LogError("No such level found.");
            }
        }

        public static void LevelCompleted(string levelId, LevelDatabase levelDatabase)
        {
            Debug.Log("in Level Completed");
            bool found = false;
            int nextIndex = 0;
            for (int i = 0; i < levelDatabase.levelDatabaseEntries.Length; i++)
            {
                LevelDatabaseEntry temp = levelDatabase.levelDatabaseEntries[i];
                if (temp.levelId == levelId)
                {
                    StaticData.LevelInformation = temp;
                    found = true;
                    nextIndex = i + 1;
                    Debug.Log("Found true");
                }
            }

            if (found)
            {
                PlayerPrefs.SetInt(levelId, 2);
                Debug.Log($"levelDataBaseEntries length = {levelDatabase.levelDatabaseEntries.Length}");
                Debug.Log($"nextIndex = {nextIndex}");
                if (nextIndex < levelDatabase.levelDatabaseEntries.Length)
                {
                    Debug.Log("setting 1 level up to unlocked");
                    PlayerPrefs.SetInt(levelDatabase.levelDatabaseEntries[nextIndex].levelId, 1);
                }
            }
            else
            {
                Debug.LogError("Level completed, but no such level found");
            }
        }

        public static void StartNewGame(LevelDatabase levelDatabase)
        {
            foreach (LevelDatabaseEntry levelDatabaseEntry in levelDatabase.levelDatabaseEntries)
            {
                PlayerPrefs.DeleteKey(levelDatabaseEntry.levelId);
            }
            PlayerPrefs.SetInt("Tutorial", 0);
            StaticData.LevelInformation = levelDatabase.levelDatabaseEntries[0];
            Debug.Log($"Starting level: {StaticData.LevelInformation.levelId}");
        }
    }
}