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
    public bool endConvo = false;
    public string nextScene;

    private int totalNumCharactersInLine;
    private int currentNumCharactersDisplayed;

    private void Start()
    {
        speakerUIs[0] = speakers[0].GetComponent<SpeakerUI>();
        speakerUIs[1] = speakers[1].GetComponent<SpeakerUI>();

        speakerUIs[0].ActiveSpeaker = conversation.speakers[0];
        speakerUIs[1].ActiveSpeaker = conversation.speakers[1];

        AdvanceConversation();
    }

    public void Update()
    {
        if (!endConvo)
        {
            if (Input.GetKeyDown("space"))
            {
                if (currentNumCharactersDisplayed < totalNumCharactersInLine)
                {
                    StopTypeWriterAndDisplayLineImmediately();
                }

                else
                {
                    AdvanceConversation();
                }
            }
        } 
        else
        {
            if (SceneManager.GetActiveScene().name != "Tutorial" && SceneManager.GetActiveScene().name != "Level1")
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void AdvanceConversation()
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
        totalNumCharactersInLine = text.Length;
        currentNumCharactersDisplayed = 0;
        StartCoroutine(EffectTypewriter(text, activeSpeakerUI));
    }

    private IEnumerator EffectTypewriter(string text, SpeakerUI speakerUI)
    {
        foreach (char c in text.ToCharArray())
        {
            speakerUI.Dialog += c;
            currentNumCharactersDisplayed+=1;
            yield return new WaitForSeconds(0.001f);
        }
    }

    void StopTypeWriterAndDisplayLineImmediately()
    {
        Line line = conversation.lines[activeLineIndex-1];
        Character character = line.character;
        StopAllCoroutines();
        
        if (speakerUIs[0].SpeakerIs(character))
        {
            speakerUIs[0].Dialog = line.text;
        }
        else
        {
            speakerUIs[1].Dialog = line.text;
        }
        currentNumCharactersDisplayed = totalNumCharactersInLine;
    }

}
