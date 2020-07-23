using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DialogueDisplay : MonoBehaviour
{
    public Conversation conversation;

    [SerializeField] private SpeakerUI speakerUI;

    private int _activeLineIndex = 0;
    public bool endConvo = false;
    public string nextScene;

    private int _totalNumCharactersInLine;
    private int _currentNumCharactersDisplayed;

    private void Start()
    {
        AdvanceConversation();
    }

    public void Update()
    {
        if (!endConvo)
        {
            if (Input.GetKeyDown("space"))
            {
                if (_currentNumCharactersDisplayed < _totalNumCharactersInLine)
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
            if (SceneManager.GetActiveScene().name != "Tutorial")
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    private void AdvanceConversation()
    {
        if (_activeLineIndex < conversation.lines.Length)
        {
            DisplayLine();
            _activeLineIndex += 1;
        }
        else
        {
            speakerUI.Hide();
            _activeLineIndex = 0;
            endConvo = true;
        }
    }    

    private void DisplayLine()
    {
        Line line = conversation.lines[_activeLineIndex];

        SetDialog(line.character, line.text);
    }

    private void SetDialog(Character character, string text)
    {
        speakerUI.ActiveSpeaker = character;
        speakerUI.Dialog = "";
        StopAllCoroutines();
        _totalNumCharactersInLine = text.Length;
        _currentNumCharactersDisplayed = 0;
        StartCoroutine(EffectTypewriter(text));
    }
    

    private IEnumerator EffectTypewriter(string text)
    {
        if (text.Length > 2 && text.Substring(0, 3).Equals("<i>"))
        {
            speakerUI.SetItalic(true);
            text = text.Substring(3);
        }
        else
        {
            speakerUI.SetItalic(false);
        }
        
        foreach (char c in text)
        {
            speakerUI.Dialog += c;
            _currentNumCharactersDisplayed+=1;
            yield return new WaitForSeconds(0.001f);
        }
    }

    void StopTypeWriterAndDisplayLineImmediately()
    {
        Line line = conversation.lines[_activeLineIndex-1];
        StopAllCoroutines();

        speakerUI.Dialog = line.text;
        
        _currentNumCharactersDisplayed = _totalNumCharactersInLine;
    }

}
