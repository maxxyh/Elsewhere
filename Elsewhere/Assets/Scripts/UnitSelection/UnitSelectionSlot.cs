using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSelectionSlot : SelectedUnitSlot, IPointerEnterHandler
{
    private static readonly int GrayscaleAmount = Shader.PropertyToID("_GrayscaleAmount");

    public event Action<SelectedUnitSlot> OnSlotMouseEnterEvent;

    public void ReAssignMaterial()
    {
        unitSprite.material = new Material(unitSprite.material);
        unitSprite.material.SetFloat(GrayscaleAmount, 0.75f);
    }
    public void SetGrayscale(bool selected)
    {
        if (selected)
        {
            unitSprite.material.SetFloat(GrayscaleAmount, 0f);
        }
        else
        {
            unitSprite.material.SetFloat(GrayscaleAmount, 0.75f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSlotMouseEnterEvent?.Invoke(this);
    }
}