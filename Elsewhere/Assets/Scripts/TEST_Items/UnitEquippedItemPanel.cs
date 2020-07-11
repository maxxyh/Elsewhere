using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitEquippedItemPanel : MonoBehaviour
{
    /*private int limit = 3;
    [SerializeField] Transform equippedItemsSlotsParent;
    public TEST_EquippedItemSlot[] equippedItemSlots;
    public event Action<TEST_Item> OnItemRightClickedEvent;*/

    public EquippedItemSlot[] equippedItemSlots;
    [SerializeField] Transform equippedItemsSlotsParent;

    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;
    public event Action<BaseItemSlot> OnRightClickEvent;
    public event Action<BaseItemSlot> OnLeftClickEvent;

    private void Start()
    {
        for (int i = 0; i < equippedItemSlots.Length; i++)
        {
            // part 15
            equippedItemSlots[i].OnPointerEnterEvent += slot => OnPointerEnterEvent(slot);
            equippedItemSlots[i].OnPointerExitEvent += slot => OnPointerExitEvent(slot);
            equippedItemSlots[i].OnRightClickEvent += slot => OnRightClickEvent(slot);
            equippedItemSlots[i].OnLeftClickEvent += slot => OnLeftClickEvent(slot);
        }
    }
    private void OnValidate()
    {
        // need to decide how many slots available (3 or 5?)
        equippedItemSlots = equippedItemsSlotsParent.GetComponentsInChildren<EquippedItemSlot>();
    }

    public bool AddItem(EquippableItem item)
    {
        for (int i = 0; i < equippedItemSlots.Length; i++)
        {
            // might not need this because the equipped slots do not have types, anything can just put in as long as the still got space
            /*if (equippedItemSlots[i].weaponType == item.weaponType)
            {*/
            if (equippedItemSlots[i].Item == null)
            {
                equippedItemSlots[i].Item = item;
                equippedItemSlots[i].Amount = 1;
                return true;
            }
            //}
        }
        return false;
    }

    public bool RemoveItem(EquippableItem item)
    {
        for (int i = 0; i < equippedItemSlots.Length; i++)
        {
            if (equippedItemSlots[i].Item == item)
            {
                // equippedItemSlots[i].equipped = false;
                equippedItemSlots[i].Item = null;
                equippedItemSlots[i].Amount = 0;
                return true;
            }
        }
        return false;
    }
}
