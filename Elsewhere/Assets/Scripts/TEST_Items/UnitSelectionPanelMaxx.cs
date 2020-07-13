using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionPanelMaxx : MonoBehaviour
{
    [HideInInspector] public List<SelectedUnitSlot> selectedUnitSlots = new List<SelectedUnitSlot>();
    [SerializeField] private GameObject unitSelectionSlotPrefab;
    
    public event Action<SelectedUnitSlot> OnSlotLeftClickEvent;
    public event Action<SelectedUnitSlot> OnSlotMouseEnterEvent;

    public void CreateUnitSelectionSlot(UnitData unitData)
    {
        GameObject slotGO = Instantiate(unitSelectionSlotPrefab, this.transform);
        UnitSelectionSlot selectedUnitSlot = slotGO.GetComponent<UnitSelectionSlot>();
        selectedUnitSlot.data = unitData;
        selectedUnitSlot.Refresh();
        selectedUnitSlot.ReAssignMaterial();
        slotGO.transform.SetAsLastSibling();
        selectedUnitSlot.OnSlotLeftClickEvent += slot => OnSlotLeftClickEvent(slot);
        selectedUnitSlot.OnSlotMouseEnterEvent += slot => OnSlotMouseEnterEvent(slot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
}
