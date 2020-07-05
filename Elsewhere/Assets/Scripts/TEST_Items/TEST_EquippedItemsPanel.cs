using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TEST_EquippedItemsPanel : MonoBehaviour
{
    private int limit = 3;
    [SerializeField] Transform equippedItemsSlotsParent;
    [SerializeField] TEST_EquippedItemSlot[] equippedItemSlots;
    public event Action<TEST_Item> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < equippedItemSlots.Length; i++)
        {
            equippedItemSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
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
            if (!equippedItemSlots[i].equipped)
            {
                equippedItemSlots[i].equipped = true;
                equippedItemSlots[i].Item = item;
                return true;
            }
            //}
        }
        Debug.Log("No more space");
        return false;
    }

    public bool RemoveItem(TEST_EquippableItem item)
    {
        for (int i = 0; i < equippedItemSlots.Length; i++)
        {
            if (equippedItemSlots[i].Item == item)
            {
                equippedItemSlots[i].equipped = false;
                equippedItemSlots[i].Item = null;
                return true;
            }
        }
        return false;
    }
}
