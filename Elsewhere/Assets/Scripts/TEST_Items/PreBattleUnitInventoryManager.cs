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
    public InventoryItemTypesManager inventoryItemTypesManager;

    [Header("Serialize Field")]
    [SerializeField] ItemSaveManager itemSaveManager;
    [SerializeField] ItemToolTip itemTooltip;

    private BaseItemSlot selectedItemSlot;

    private void OnValidate()
    {

        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemToolTip>();
    }

    public void Awake()
    {
        // Setup Events
        // Pointer Enter
        inventory.OnPointerEnterEvent += ShowTooltip;
        unitPersonalInventory.OnPointerEnterEvent += ShowTooltip;

        // Pointer Exit
        inventory.OnPointerExitEvent += HideTooltip;
        unitPersonalInventory.OnPointerExitEvent += HideTooltip;

        // Left Click
        inventory.OnLeftClickEvent += InventoryLeftClick;
        unitPersonalInventory.OnLeftClickEvent += UnitPersonalInventoryLeftClick;

    }

    private void Start()
    {
        /*if (itemSaveManager != null)
        {
            itemSaveManager.LoadUnitData(this.unit.unitID);
        }*/
    }

    /*private void OnDestroy()
    {
        *//*if (!IsInventoryNull() && !IsEquippedPanelNull())
        {
            itemSaveManager.SaveInventory(this);
            itemSaveManager.SaveEquippedItems(this);
        }
        else
        {
            Debug.Log("Null destroy inventory");
        }*//*
    }*/

    private void OnDestroy()
    {
        
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
                    inventoryItemTypesManager.currInventoryItemsList.Remove(currItem);
                }
                else
                {
                    Debug.Log("No more space in unit");
                    inventory.AddItem(currItem);
                }
            }
        }
    }

    private void UnitPersonalInventoryLeftClick(BaseItemSlot itemSlot)
    {
        if (unit != null)
        {
            selectedItemSlot = itemSlot;
            if (itemSlot.Item != null)
            {
                Item item = itemSlot.Item;
                if (inventory.AddItem(item) && unitPersonalInventory.RemoveItem(item))
                {
                    // equippedItemsPanel.RemoveItem(equippableItem);
                    // inventory.AddItem(item);
                    // unitPersonalInventory.RemoveItem(item);
                    unit.unitItems.Remove(item);
                    inventoryItemTypesManager.currInventoryItemsList.Add(item);
                }
                else
                {
                    // unitPersonalInventory.AddItem(item);
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
