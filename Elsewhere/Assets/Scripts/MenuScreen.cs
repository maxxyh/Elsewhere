using LevelSelection;
using UnityEngine;

namespace DefaultNamespace
{
    public class MenuScreen : MonoBehaviour
    {
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private ChangeScene changeSceneManager;

        public void OnSavedGameButton()
        {
            if (PlayerPrefs.HasKey("Tutorial") && PlayerPrefs.GetInt("Tutorial") == 2)
            {
                changeSceneManager.OnlickChangeSceneButton("NavigationMap");
            }
        }

        public void OnStartNewGame()
        {
            LevelSelectManager.StartNewGame(levelDatabase);
        }
    }
}