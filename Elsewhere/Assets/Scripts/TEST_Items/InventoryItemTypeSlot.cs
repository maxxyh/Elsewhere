using NSubstitute.Core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemTypeSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] ItemType itemType;

    public event Action<InventoryItemTypeSlot> OnSlotClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClick?.Invoke(this);
    }

    public ItemType GetSlotType()
    {
        return this.itemType;
    }
}
