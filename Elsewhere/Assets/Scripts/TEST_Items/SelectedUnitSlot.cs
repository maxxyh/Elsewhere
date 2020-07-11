using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedUnitSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Text unitName;
    [SerializeField] Image unitSprite;
    public UnitData data;

    public event Action<SelectedUnitSlot> OnSlotLeftClickEvent;

    private void Awake()
    {
        unitSprite.sprite = data.unitSprite;
        unitName.text = data.unitID;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotLeftClickEvent?.Invoke(this);
    }
}
