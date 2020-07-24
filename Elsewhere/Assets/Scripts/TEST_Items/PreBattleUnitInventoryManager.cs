using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PreBattleUnitInventoryManager : MonoBehaviour
{
    [Header("Unit")]
    public UnitData unit;

    [Header("Public")]
    public InfiniteInventory commonInventory;
    public UnitPersonalInventory unitPersonalInventory;
    public InventoryItemTypesManager inventoryItemTypesManager;

    
    [Header("Serialize Field")]
    [FormerlySerializedAs("itemSaveManager")] [SerializeField] UnitSaveManager unitSaveManager;
    [SerializeField] ItemToolTip itemTooltip;

    private BaseItemSlot selectedItemSlot;

    private void OnValidate()
    {
        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemToolTip>();
        commonInventory.SetStartingItems(unitSaveManager.LoadInventory());
    }

    public void Awake()
    {
        commonInventory.SetStartingItems(unitSaveManager.LoadInventory());

        // Setup Events
        // Pointer Enter
        commonInventory.OnPointerEnterEvent += ShowTooltip;
        unitPersonalInventory.OnPointerEnterEvent += ShowTooltip;

        // Pointer Exit
        commonInventory.OnPointerExitEvent += HideTooltip;
        unitPersonalInventory.OnPointerExitEvent += HideTooltip;

        // Left Click
        commonInventory.OnLeftClickEvent += InventoryLeftClick;
        unitPersonalInventory.OnLeftClickEvent += UnitPersonalInventoryLeftClick;

    }

    private void Start()
    {
        /*if (itemSaveManager != null)
        {
            itemSaveManager.LoadUnitData(this.unit.unitID);
        }*/
    }

    private void OnDestroy()
    {
        unitSaveManager.SaveInventory(commonInventory.ItemSlots);
    }


    /*private void OnDestroy()
    {
        if (!IsInventoryNull() && !IsEquippedPanelNull())
        {
            itemSaveManager.SaveInventory(this);
            itemSaveManager.SaveEquippedItems(this);
        }
        else
        {
            Debug.Log("Null destroy inventory");
        }
    }*/

    private bool IsInventoryNull()
    {
        for (int i = 0; i < commonInventory.ItemSlots.Count; i++)
        {
            if (commonInventory.ItemSlots[i].Item != null)
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
        Debug.Log("CLICKED");
        if (unit != null)
        {
            selectedItemSlot = itemSlot;
            if (itemSlot.Item != null)
            {
                Item currItem = itemSlot.Item;
                if (commonInventory.RemoveItem(itemSlot.Item) && unitPersonalInventory.AddItem(currItem))
                { 
                    unit.unitItems.Add(currItem);
                    inventoryItemTypesManager.currInventoryItemsList.Remove(currItem);
                }
                else
                {
                    Debug.Log("No more space in unit");
                    commonInventory.AddItem(currItem);
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
                if (unitPersonalInventory.RemoveItem(item) && commonInventory.AddItem(item)) //
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
