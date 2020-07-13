using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedUnitSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Text unitName;
    [SerializeField] protected Image unitSprite;
    public UnitData data;

    [HideInInspector]
    public string UnitName => unitName.text;

    public event Action<SelectedUnitSlot> OnSlotLeftClickEvent;

    private void Start()
    {
        if (data != null)
        {
            unitSprite.sprite = data.unitSprite;
            unitName.text = data.unitID;
        }
        else
        {
            Debug.LogError("selectedUnitSlot data not initialised on start");
        }
        
    }

    public void Refresh()
    {
        unitSprite.sprite = data.unitSprite;
        unitName.text = data.unitID;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotLeftClickEvent?.Invoke(this);
    }
}
