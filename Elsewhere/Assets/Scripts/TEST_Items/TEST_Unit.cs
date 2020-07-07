using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TEST_Unit : MonoBehaviour
{
    public int health = 50;

    [Header("Stats")]
    public UnitStat physicalAttack;
    public UnitStat magicalAttack;
    public UnitStat hitRate;
    public UnitStat critRate;
    public UnitStat attackRange;

    [Header("Public")]
    public TEST_Inventory inventory;
    public TEST_EquippedItemsPanel equippedItemsPanel;

    [Header("Serialize Field")]
    [SerializeField] TEST_StatPanel statPanel;
    [SerializeField] ItemSaveManager itemSaveManager;
    [SerializeField] TEST_ItemToolTip itemTooltip;

    private TEST_BaseItemSlot selectedItemSlot;

    private void OnValidate()
    {
        UnitStat physicalAttack = new UnitStat(25);
        UnitStat magicalAttack = new UnitStat(12);
        UnitStat hitRate = new UnitStat(0, true);
        UnitStat critRate = new UnitStat(30, true);
        UnitStat attackRange = new UnitStat(3);
        this.physicalAttack = physicalAttack;
        this.magicalAttack = magicalAttack;
        this.hitRate = hitRate;
        this.critRate = critRate;
        this.attackRange = attackRange;


        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<TEST_ItemToolTip>();
    }

    public void Awake()
    {
        statPanel.SetStats(this.physicalAttack, this.magicalAttack, this.hitRate, this.critRate, this.attackRange);
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

    }

    private void Start()
    {
        if (itemSaveManager != null)
        {
            itemSaveManager.LoadInventory(this);
            itemSaveManager.LoadEquippedItems(this);
        }
    }

    private void OnDestroy()
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

    // item in inventory can be some other type, not nec equippable so need to check if its equippable only then will equip
    private void InventoryRightClick(TEST_BaseItemSlot itemSlot)
    {
        selectedItemSlot = itemSlot;
        // Debug.Log(itemSlot.Item);
        // AfterInventoryRightClickEvent();
        if (itemSlot.Item != null)
        {
            if (itemSlot.Item is TEST_EquippableItem)
            {
                Equip((TEST_EquippableItem)itemSlot.Item);
            }
            else if (itemSlot.Item is TEST_UsableItem)
            {
                TEST_UsableItem usableItem = (TEST_UsableItem)itemSlot.Item;
                usableItem.Use(this);

                if (usableItem.IsConsumable)
                {
                    itemSlot.Amount--;
                    // inventory.RemoveItem(usableItem);
                    usableItem.Destroy();
                }
            }
        }
        AfterInventoryRightClickEvent();
    }

    private void EquippedItemPanelRightClick(TEST_BaseItemSlot itemSlot)
    {
        Debug.Log(itemSlot.Item);
        selectedItemSlot = itemSlot;
        if (itemSlot.Item != null)
        {
            if (itemSlot.Item is TEST_EquippableItem)
            {
                Unequip((TEST_EquippableItem)itemSlot.Item);
            }
        }
        AfterInventoryRightClickEvent();
    }

    private void ShowTooltip(TEST_BaseItemSlot itemSlot)
    {
        if (itemSlot.Item != null)
        {
            itemTooltip.ShowToolTip(itemSlot.Item);
        }
    }

    private void HideTooltip(TEST_BaseItemSlot itemSlot)
    {
        if (itemTooltip.gameObject.activeSelf)
        {
            itemTooltip.HideToolTip();
        }
    }


    public void Equip(TEST_EquippableItem item)
    {
        // check if can remove item from inventory
        if (inventory.RemoveItem(item))
        {
            if (equippedItemsPanel.AddItem(item))
            {
                item.Equip(this);
                statPanel.UpdateStatValues();
            }
        }
        else
        {
            inventory.AddItem(item);
            Debug.Log("Remove items from unit to get more space");
        }
    }

    public void Unequip(TEST_EquippableItem item)
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

    private void AfterInventoryRightClickEvent()
    {
        for (int i = 0; i < equippedItemsPanel.equippedItemSlots.Length; i++)
        {
            TEST_EquippedItemSlot currSlot = equippedItemsPanel.equippedItemSlots[i];
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

    private void AddStacks(TEST_BaseItemSlot destinationSlot)
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
