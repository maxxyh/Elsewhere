using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TEST_EquippedItemsPanel : MonoBehaviour
{
    /*private int limit = 3;
    [SerializeField] Transform equippedItemsSlotsParent;
    public TEST_EquippedItemSlot[] equippedItemSlots;
    public event Action<TEST_Item> OnItemRightClickedEvent;*/

    public TEST_EquippedItemSlot[] equippedItemSlots;
    [SerializeField] Transform equippedItemsSlotsParent;

    public event Action<TEST_BaseItemSlot> OnPointerEnterEvent;
    public event Action<TEST_BaseItemSlot> OnPointerExitEvent;
    public event Action<TEST_BaseItemSlot> OnRightClickEvent;

    private void Start()
    {
        for (int i = 0; i < equippedItemSlots.Length; i++)
        {
            equippedItemSlots[i].OnPointerEnterEvent += slot => OnPointerEnterEvent(slot);
            equippedItemSlots[i].OnPointerExitEvent += slot => OnPointerExitEvent(slot);
            equippedItemSlots[i].OnRightClickEvent += slot => OnRightClickEvent(slot);
        }
    }
    private void OnValidate()
    {
        // need to decide how many slots available (3 or 5?)
        equippedItemSlots = equippedItemsSlotsParent.GetComponentsInChildren<TEST_EquippedItemSlot>();
    }

    public bool AddItem(TEST_EquippableItem item)
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

    public bool RemoveItem(TEST_EquippableItem item)
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
