using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemTypesPanel : MonoBehaviour
{
    [SerializeField] InventoryItemTypeSlot[] inventoryItemTypeSlots;
    [SerializeField] Transform inventoryTypeSlotParent;
    
    public event Action<InventoryItemTypeSlot> OnSlotClick;

    private void OnValidate()
    {
        inventoryItemTypeSlots = inventoryTypeSlotParent.GetComponentsInChildren<InventoryItemTypeSlot>();
    }

    private void Start()
    {
        foreach(InventoryItemTypeSlot slot in inventoryItemTypeSlots)
        {
            slot.OnSlotClick += slotParam => OnSlotClick(slotParam);
        }
    }
}
