using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public void OnlickChangeSceneButton(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
