using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueDisplay : MonoBehaviour
{
    public Conversation conversation;

    public GameObject[] speakers;
    public SpeakerUI[] speakerUIs;

    private int activeLineIndex = 0;
    private bool endConvo = false;
    public string nextScene;

    private void Start()
    {
        speakerUIs[0] = speakers[0].GetComponent<SpeakerUI>();
        speakerUIs[1] = speakers[1].GetComponent<SpeakerUI>();

        speakerUIs[0].ActiveSpeaker = conversation.speakers[0];
        speakerUIs[1].ActiveSpeaker = conversation.speakers[1];
    }

    private void Update()
    {
        if (!endConvo)
        {
            if (Input.GetKeyDown("space"))
            {
                AdvanceConversation();
            }
        } 
        else
        {
            if (SceneManager.GetActiveScene().name != "Tutorial")
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    void AdvanceConversation()
    {
        if (activeLineIndex < conversation.lines.Length)
        {
            DisplayLine();
            activeLineIndex += 1;
        }
        else
        {
            foreach (SpeakerUI speakerUI in speakerUIs)
            {
                speakerUI.Hide();
            }
            activeLineIndex = 0;
            endConvo = true;
        }
    }    

    void DisplayLine()
    {
        Line line = conversation.lines[activeLineIndex];
        Character character = line.character;

        if (speakerUIs[0].SpeakerIs(character))
        {
            SetDialog(speakerUIs[0], speakerUIs[1], line.text);
        } 
        else
        {
            SetDialog(speakerUIs[1], speakerUIs[0], line.text);
        }    
    }

    void SetDialog(SpeakerUI activeSpeakerUI, SpeakerUI inactiveSpeakerUI, string text)
    {
        activeSpeakerUI.Show();
        inactiveSpeakerUI.Hide();
        activeSpeakerUI.Dialog = "";
        StopAllCoroutines();
        StartCoroutine(EffectTypewriter(text, activeSpeakerUI));
    }

    private IEnumerator EffectTypewriter(string text, SpeakerUI speakerUI)
    {
        foreach (char c in text.ToCharArray())
        {
            speakerUI.Dialog += c;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
