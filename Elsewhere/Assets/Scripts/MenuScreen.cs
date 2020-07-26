using LevelSelection;
using UnityEngine;

namespace DefaultNamespace
{
    public class MenuScreen : MonoBehaviour
    {
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private ChangeScene changeSceneManager;
        [SerializeField] private UnitSaveManager unitSaveManager;

        public void OnSavedGameButton()
        {
            if (PlayerPrefs.HasKey("Tutorial") && PlayerPrefs.GetInt("Tutorial") == 2)
            {
                changeSceneManager.OnlickChangeSceneButton("NavigationMap");
            }
        }

        public void OnStartNewGame()
        {
            unitSaveManager.ResetUnitAndInventorySaveData();
            LevelSelectManager.StartNewGame(levelDatabase);
        }
    }
}