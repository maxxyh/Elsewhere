using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriterEffect : MonoBehaviour
{
    private float speed = 0.02f;
    public string fullText;
    private string currText = "";
    void Start()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            currText = fullText.Substring(0, i);
            this.GetComponent<Text>().text = currText; 
            yield return new WaitForSeconds(speed);
        }
    }
}
