using UnityEngine;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public string sceneToLoad;
    private float timer = 1f;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
