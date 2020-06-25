using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MajorStatPanel : StatPanel
{
    public Button closeButton;
    public List<Text> skillManaCost = new List<Text>();

    public void OnCloseButton()
    {
        this.gameObject.SetActive(false);
    }

    public void AssignManaCost(List<Ability> abilities)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            int manaCost = abilities[i].GetManaCost();
            skillManaCost[i].text = manaCost.ToString();
        }
    }
}
