using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerUI : MonoBehaviour
{
    public Image portrait;
    public Text fullName;
    public Text dialogue;

    private Character activeSpeaker;
    public Character ActiveSpeaker
    {
        get { return activeSpeaker; }
        set
        {
            activeSpeaker = value;
            portrait.sprite = activeSpeaker.portrait;
            fullName.text = activeSpeaker.fullName;
        }
    }

    public string Dialog
    {
        get { return dialogue.text; }
        set { dialogue.text = value; }
    }

    public bool HasSpeaker()
    {
        return activeSpeaker != null;
    }

    public bool SpeakerIs(Character character)
    {
        return activeSpeaker == character;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
