using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreBattleUnitInventoryManager : MonoBehaviour
{
    [Header("Unit")]
    public UnitData unit;

    [Header("Public")]
    public CommonInventory inventory;
    public UnitPersonalInventory unitPersonalInventory;

    [Header("Serialize Field")]
    [SerializeField] public ItemSaveManager itemSaveManager;
    [SerializeField] ItemToolTip itemTooltip;

    private BaseItemSlot selectedItemSlot;

    private void OnValidate()
    {

        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemToolTip>();
    }

    // Load unit inventory
    // turnscheduler has event invoke sent Unit/UnitInventory on unit selected;

    public void Awake()
    {
        // setup Events"

        // Pointer Enter
        inventory.OnPointerEnterEvent += ShowTooltip;
        unitPersonalInventory.OnPointerEnterEvent += ShowTooltip;

        // Pointer Exit
        inventory.OnPointerExitEvent += HideTooltip;
        unitPersonalInventory.OnPointerExitEvent += HideTooltip;

        // Left Click
        inventory.OnLeftClickEvent += InventoryLeftClick;
        unitPersonalInventory.OnLeftClickEvent += EquippedItemsPanelLeftClick;

    }

    private void Start()
    {
        /*if (itemSaveManager != null)
        {
            itemSaveManager.LoadInventory(this);
            itemSaveManager.LoadEquippedItems(this);
        }*/
    }

    private void OnDestroy()
    {
        /*if (!IsInventoryNull() && !IsEquippedPanelNull())
        {
            itemSaveManager.SaveInventory(this);
            itemSaveManager.SaveEquippedItems(this);
        }
        else
        {
            Debug.Log("Null destroy inventory");
        }*/
    }

    private bool IsInventoryNull()
    {
        for (int i = 0; i < inventory.ItemSlots.Count; i++)
        {
            if (inventory.ItemSlots[i].Item != null)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsEquippedPanelNull()
    {
        for (int i = 0; i < unitPersonalInventory.ItemSlots.Count; i++)
        {
            if (unitPersonalInventory.ItemSlots[i].Item != null)
            {
                return false;
            }
        }
        return true;
    }

    private void InventoryLeftClick(BaseItemSlot itemSlot)
    {
        if (unit != null)
        {
            selectedItemSlot = itemSlot;

            if (itemSlot.Item != null)
            {
                Item currItem = itemSlot.Item;
                if (inventory.RemoveItem(itemSlot.Item) && unitPersonalInventory.AddItem(currItem))
                {
                    unit.unitItems.Add(currItem);
                }
                else
                {
                    Debug.Log("No more space in unit");
                    inventory.AddItem(currItem);
                }
            }
        }
    }
    private void EquippedItemsPanelLeftClick(BaseItemSlot itemSlot)
    {
        if (unit != null)
        {
            selectedItemSlot = itemSlot;
            if (itemSlot.Item != null)
            {
                EquippableItem equippableItem = (EquippableItem)itemSlot.Item;
                if (inventory.CanAddItem(itemSlot.Item) && equippedItemsPanel.RemoveItem(equippableItem))
                {
                    // equippedItemsPanel.RemoveItem(equippableItem);
                    inventory.AddItem(equippableItem);
                    equippedItemsPanel.RemoveItem(equippableItem);
                    unit.unitItems.Remove(equippableItem);
                }
                else
                {
                    equippedItemsPanel.AddItem(equippableItem);
                }
            }
        }
    }


    private void ShowTooltip(BaseItemSlot itemSlot)
    {
        if (itemSlot.Item != null)
        {
            itemTooltip.ShowToolTip(itemSlot.Item);
        }
    }

    private void HideTooltip(BaseItemSlot itemSlot)
    {
        if (itemTooltip.gameObject.activeSelf)
        {
            itemTooltip.HideToolTip();
        }
    }
}
