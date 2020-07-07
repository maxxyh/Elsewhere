using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TEST_StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text nameText;
    [SerializeField] Text valueText;
    [SerializeField] TEST_StatToolTip toolTip;
    private bool showingTooltip;

    private UnitStat _stat;
    public UnitStat Stat
    {
        get { return _stat; }
        set
        {
            _stat = value;
            UpdateStatValue();
        }
    }

    private void OnValidate()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        nameText = texts[0];
        valueText = texts[1];
        if (toolTip == null)
        {
            toolTip = FindObjectOfType<TEST_StatToolTip>();
        }
    }

    public void UpdateStatValue()
    {
        valueText.text = _stat.Value.ToString();
        if (showingTooltip)
        {
            toolTip.ShowToolTip(Stat, Name);
        }
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            nameText.text = _name;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.ShowToolTip(Stat, Name);
        showingTooltip = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.HideToolTip();
        showingTooltip = false;
    }
}
