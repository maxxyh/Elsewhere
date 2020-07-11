using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InBattleUnitInventoryManager : MonoBehaviour
{
    [Header("Unit")] 
    public UnitData unit;
    
    [Header("Public")]
    public CommonInventory inventory;
    public UnitEquippedItemPanel equippedItemsPanel;

    [Header("Serialize Field")]
    [SerializeField] public InventoryStatPanel statPanel;
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
        statPanel.SetStats(new UnitStat(0), new UnitStat(0), new UnitStat(0), new UnitStat(0), new UnitStat(0));
        statPanel.UpdateStatValues();

        // setup Events"
        // RightClick
        inventory.OnRightClickEvent += InventoryRightClick;
        equippedItemsPanel.OnRightClickEvent += EquippedItemPanelRightClick;

        // Pointer Enter
        inventory.OnPointerEnterEvent += ShowTooltip;
        equippedItemsPanel.OnPointerEnterEvent += ShowTooltip;

        // Pointer Exit
        inventory.OnPointerExitEvent += HideTooltip;
        equippedItemsPanel.OnPointerExitEvent += HideTooltip;

        // Left Click
        inventory.OnLeftClickEvent += InventoryLeftClick;
        equippedItemsPanel.OnLeftClickEvent += EquippedItemsPanelLeftClick;

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
        for (int i = 0; i < equippedItemsPanel.equippedItemSlots.Length; i++)
        {
            if (equippedItemsPanel.equippedItemSlots[i].Item != null)
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
                if (itemSlot.Item is EquippableItem)
                {
                    EquippableItem equippableItem = (EquippableItem)itemSlot.Item;
                    if (inventory.RemoveItem(itemSlot.Item) && equippedItemsPanel.AddItem(equippableItem))
                    {
                        unit.unitItems.Add(equippableItem);
                    }
                    else
                    {
                        Debug.Log("No more space in unit");
                        inventory.AddItem(equippableItem);
                    }
                }
                else
                {
                    // add consumable to the unit inventory (dont use)
                }
            }
        }
        // AfterRightClickEvent();
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

    // item in inventory can be some other type, not nec equippable so need to check if its equippable only then will equip
    private void InventoryRightClick(BaseItemSlot itemSlot)
    {
        selectedItemSlot = itemSlot;
        // Debug.Log(itemSlot.Item);
        // AfterInventoryRightClickEvent();
        if (itemSlot.Item != null)
        {
            if (itemSlot.Item is EquippableItem)
            {
                Equip((EquippableItem)itemSlot.Item);
            }
            else if (itemSlot.Item is UsableItem)
            {
                UsableItem usableItem = (UsableItem)itemSlot.Item;
                usableItem.Use(this);

                if (usableItem.IsConsumable)
                {
                    itemSlot.Amount--;
                    // inventory.RemoveItem(usableItem);
                    usableItem.Destroy();
                }
            }
        }
        AfterRightClickEvent();
    }

    private void EquippedItemPanelRightClick(BaseItemSlot itemSlot)
    {
        selectedItemSlot = itemSlot;
        if (itemSlot.Item != null)
        {
            if (itemSlot.Item is EquippableItem)
            {
                Unequip((EquippableItem)itemSlot.Item);
            }
        }
        AfterRightClickEvent();
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

    public void Equip(EquippableItem item)
    {
        // check if can remove item from inventory
        if (inventory.RemoveItem(item) && equippedItemsPanel.AddItem(item))
        {
            item.Equip(this);
            statPanel.UpdateStatValues();
        }
        else
        {
            inventory.AddItem(item);
            Debug.Log("Remove items from unit to get more space");
        }
    }

    public void Unequip(EquippableItem item)
    {
        if (inventory.CanAddItem(item) && equippedItemsPanel.RemoveItem(item))
        {
            item.Unequip(this);
            statPanel.UpdateStatValues();
            inventory.AddItem(item);
            equippedItemsPanel.RemoveItem(item);
        }
    }

    public void UpdateStatValues()
    {
        // then call this in StatBuff item effect
        statPanel.UpdateStatValues();
    }

    private void AfterRightClickEvent()
    {
        for (int i = 0; i < equippedItemsPanel.equippedItemSlots.Length; i++)
        {
            EquippedItemSlot currSlot = equippedItemsPanel.equippedItemSlots[i];
            if (selectedItemSlot == null)
            {
                Debug.Log("TEST_Unit currSlot aft inventory right click is null");
                return;
            }
            // Debug.Log("is currSlot null? " + selectedItemSlot.Item == null);
            if (selectedItemSlot.Item == null)
            {
                return;
            }
            if (currSlot.Item == null || currSlot.CanAddStack(selectedItemSlot.Item))
            {
                
                AddStacks(currSlot);
            }
        }
    }

    private void AddStacks(BaseItemSlot destinationSlot)
    {
        int numAddableStacks = destinationSlot.Item.maxStack - destinationSlot.Amount;
        int stacksToAdd = Mathf.Min(numAddableStacks, selectedItemSlot.Amount);

        destinationSlot.Amount += stacksToAdd;
        selectedItemSlot.Amount -= stacksToAdd;
    }

    /*private ItemContainer openItemContainer;

    // for item stash
    private void TransferToItemContainer(TEST_ItemSlot itemSlot)
    {
        TEST_Item item = itemSlot.Item;
        if (item != null && openItemContainer.CanAddItem(item))
        {
            inventory.RemoveItem(item);
            openItemContainer.AddItem(item);
        }
    }

    private void TransferToInventory(TEST_ItemSlot itemSlot)
    {
        TEST_Item item = itemSlot.Item;
        if (item != null && inventory.CanAddItem(item))
        {
            openItemContainer.RemoveItem(item);
            inventory.AddItem(item);
        }
    }

    public void OpenItemContainer(ItemContainer itemContainer)
    {
        openItemContainer = itemContainer;

        inventory.OnRightClickEvent -= InventoryRightClick;
        inventory.OnRightClickEvent += TransferToItemContainer;

        itemContainer.OnRightClickEvent += TransferToInventory;

        itemContainer.OnPointerEnterEvent += ShowTooltip;
        itemContainer.OnPointerExitEvent += HideTooltip;
    }

    public void CloseItemContainer(ItemContainer itemContainer)
    {
        openItemContainer = null;

        inventory.OnRightClickEvent += InventoryRightClick;
        inventory.OnRightClickEvent -= TransferToItemContainer;

        itemContainer.OnRightClickEvent -= TransferToInventory;

        itemContainer.OnPointerEnterEvent -= ShowTooltip;
        itemContainer.OnPointerExitEvent -= HideTooltip;
    }*/
}
