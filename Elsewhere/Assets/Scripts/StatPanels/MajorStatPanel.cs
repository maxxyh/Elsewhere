using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MajorStatPanel : StatPanel
{
    public Button closeButton;
    public List<Text> skillManaCost = new List<Text>();
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private GameObject abilitySlotPrefab;
    [SerializeField] private ProgressBar progressBar;

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

    public void AddAbilityToPanel(string name, string description, int cost)
    {
        GameObject abilitySlotGO = Instantiate(abilitySlotPrefab, skillPanel.transform);
        AbilitySlot abilitySlot = abilitySlotGO.GetComponent<AbilitySlot>();
        abilitySlot.Initialize(name,description,cost.ToString());
        abilitySlot.transform.SetAsLastSibling();
        LayoutRebuilder.ForceRebuildLayoutImmediate(skillPanel.GetComponent<RectTransform>());
    }

    public void ClearAllAbilitiesFromPanel()
    {
        for (int i = 0; i < skillPanel.transform.childCount; i++)
        {
            Destroy(skillPanel.transform.GetChild(i).gameObject);
        }
    }

    public override void UpdateLevel(Level level)
    {
        progressBar.SetCurrentFill(level.currentExperience, level.requiredExperience, level.currentLevel);
    }
}
