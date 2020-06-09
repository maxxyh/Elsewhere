using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesPanel : MonoBehaviour
{
    public GameObject playerActionPanel;
    public GameObject abilitiesPanel;
    public PlayerUnit currPlayer;
    private Button skill1Button;
    private Button skill2Button;
    private Text text1;
    private Text text2;

    private void Awake()
    {
        text1 = skill1Button.GetComponentInChildren<Text>();
        text2 = skill2Button.GetComponentInChildren<Text>();
    }

    public void OnSkill1Button()
    {
        Debug.Log("skill1 pressed");
        StartCoroutine(currPlayer.abilities[0].Execute());
    }

    public void OnSkill2Button()
    {
        Debug.Log("skill2 pressed");
        StartCoroutine(currPlayer.abilities[1].Execute());
    }

    public void OnReturnButton()
    {
        playerActionPanel.SetActive(true);
        abilitiesPanel.SetActive(false);
    }
    public void OnAbilitiesButton()
    {
        abilitiesPanel.SetActive(true);
        text1.text = currPlayer.abilities[0].abilityName;
        text2.text = currPlayer.abilities[1].abilityName;
        playerActionPanel.SetActive(false); 
    }

    
}
