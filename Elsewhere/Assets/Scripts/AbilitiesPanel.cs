using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesPanel : MonoBehaviour
{
    public TurnScheduler turnScheduler;
    public GameObject playerActionPanel;
    public GameObject abilitiesPanel;
    public Button skill1Button;
    public Button skill2Button;
    private Text text1;
    private Text text2;

    private void Awake()
    {
        text1 = skill1Button.GetComponentInChildren<Text>();
        text2 = skill2Button.GetComponentInChildren<Text>();
        abilitiesPanel.SetActive(false);
    }

    
    public void OnSkill1Button()
    {
        Debug.Log("skill1 pressed");
        turnScheduler.OnAbilityButton(turnScheduler.currUnit.abilities[0]);
    }

    public void OnSkill2Button()
    {
        Debug.Log("skill2 pressed");
        turnScheduler.OnAbilityButton(turnScheduler.currUnit.abilities[1]);
    }
    

    public void OnReturnButton()
    {
        playerActionPanel.SetActive(true);
        abilitiesPanel.SetActive(false);
    }
    public void OnAbilitiesButton()
    {
        abilitiesPanel.SetActive(true);
        text1.text = turnScheduler.currUnit.abilities[0].abilityName;
        text2.text = turnScheduler.currUnit.abilities[1].abilityName;
        playerActionPanel.SetActive(false); 
    }

    
}
