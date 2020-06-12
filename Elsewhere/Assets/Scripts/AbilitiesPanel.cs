using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesPanel : MonoBehaviour
{
    public TurnScheduler turnScheduler;
    public GameObject mainActionPanel;
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

        Ability ability = turnScheduler.currUnit.abilities[0];
        CheckEnoughMana(turnScheduler.currUnit, ability);

        //abilitiesPanel.SetActive(false);
        //undoPanel.SetActive(true);

        turnScheduler.OnAbilityButton(ability);
    }

    public void OnSkill2Button()
    {
        Debug.Log("skill2 pressed");

        Ability ability = turnScheduler.currUnit.abilities[1];
        CheckEnoughMana(turnScheduler.currUnit, ability);

        //abilitiesPanel.SetActive(false);
        //undoPanel.SetActive(true);

        turnScheduler.OnAbilityButton(ability);
    }
    

    public void OnReturnButton()
    {
        mainActionPanel.SetActive(true);
        abilitiesPanel.SetActive(false);
        turnScheduler.OnExitAbilityMenuButton();
    }
    public void OnAbilitiesButton()
    {
        abilitiesPanel.SetActive(true);
        Debug.Log("Player Ability size inside call " + turnScheduler.currUnit.abilities.Count);
        text1.text = turnScheduler.currUnit.abilities[0].abilityName;
        text2.text = turnScheduler.currUnit.abilities[1].abilityName;
        mainActionPanel.SetActive(false);
        turnScheduler.OnAbilityMenuButton();
    }

    
    private static bool CheckEnoughMana(Unit unit, Ability ability)
    {
        return unit.stats["mana"].Value >= ability.GetManaCost();
    }

}
