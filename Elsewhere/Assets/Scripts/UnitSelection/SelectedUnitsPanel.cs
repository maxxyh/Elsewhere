﻿using System;
using UnityEngine;

public class SelectedUnitsPanel : MonoBehaviour
{
    public SelectedUnitSlot[] selectedUnitSlots;
    [SerializeField] Transform selectedUnitParent;

    public event Action<SelectedUnitSlot> OnSlotLeftClickEvent;
    private void OnValidate()
    {
        selectedUnitSlots = selectedUnitParent.GetComponentsInChildren<SelectedUnitSlot>();
    }

    private void Start()
    {
        for (int i = 0; i < selectedUnitSlots.Length; i++)
        {
            selectedUnitSlots[i].OnSlotLeftClickEvent += slot => OnSlotLeftClickEvent(slot);
        }
    }
}
