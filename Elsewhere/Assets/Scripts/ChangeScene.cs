using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public Animator loadEffect;

    public void OnlickChangeSceneButton(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void OnClickQuitButton()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    public void OnFeedbackButton()
    {
        Application.OpenURL("https://forms.gle/HCzFuG7pJXH3ed2K7");
    }

    IEnumerator LoadScene(string sceneName)
    {
        loadEffect.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator CrossFade()
    {
        loadEffect.SetTrigger("Start");
        yield return new WaitForSeconds(0.4f);
    }
}
