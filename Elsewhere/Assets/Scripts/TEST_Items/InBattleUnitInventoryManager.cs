using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InBattleUnitInventoryManager : MonoBehaviour
{
    [Header("Unit")] 
    public Unit unit;
    
    [Header("Public")]
    public UnitPersonalInventory unitPersonalInventory;
    // public UnitEquippedItemPanel equippedItemsPanel;

    [Header("Serialize Field")]
    [SerializeField] public InventoryStatPanel statPanel;
    [SerializeField] ItemToolTip itemTooltip;
    [SerializeField] private Text unitName;
    [SerializeField] private Image unitSprite;
    [SerializeField] private GameObject usableItemConfirmationPanel;
    [SerializeField] private GameObject playerInventoryPanel;
    

    private BaseItemSlot selectedItemSlot;
    private Item previousItem;
    private BaseItemSlot currUsableItemSlot;

    public static Action OnUsedUsableItem;

    private void OnValidate()
    {

        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemToolTip>();
    }

    // Load unit inventory
    // turnscheduler has event invoke sent Unit/UnitInventory on unit selected;

    public void Awake()
    {
        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemToolTip>();
        // setup Events
        // Pointer Enter
        unitPersonalInventory.OnPointerEnterEvent += ShowTooltip;

        // Pointer Exit
        unitPersonalInventory.OnPointerExitEvent += HideTooltip;

        // Left Click
        unitPersonalInventory.OnLeftClickEvent += PersonalInventoryLeftClick;
    }

    private static UnitStat[] GetStatsToDisplay(Unit unit)
    {
        UnitStat[] stats = new UnitStat[5];
        stats[0] = unit.stats[StatString.PHYSICAL_DAMAGE];
        stats[1] = unit.stats[StatString.MAGIC_DAMAGE];
        stats[2] = unit.stats[StatString.HIT_RATE];
        stats[3] = unit.stats[StatString.CRIT_RATE];
        stats[4] = unit.stats[StatString.ATTACK_RANGE];
        return stats;
    }

    private void Start()
    {
        /*if (itemSaveManager != null)
        {
            itemSaveManager.LoadInventory(this.unit);
            //itemSaveManager.LoadEquippedItems(this);
        }*/
    }

    #region deprecated

    /*    private bool IsInventoryNull()
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
        }*/
    
    /*    private void InventoryLeftClick(BaseItemSlot itemSlot)
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
        }*/
    /*    private void EquippedItemsPanelLeftClick(BaseItemSlot itemSlot)
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
        }*/
    
     /*   private void EquippedItemPanelRightClick(BaseItemSlot itemSlot)
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
        }*/


    #endregion


    private void PersonalInventoryLeftClick(BaseItemSlot itemSlot)
    {
        selectedItemSlot = itemSlot;
        ItemSlotData previousItemSlotData = unit.unitInventory.Find(x =>
        {
            if (x.Item is EquippableItem)
            {
                return ((EquippableItem) x.Item).equipped;
            }

            return false;
        });
        if (unit == null || itemSlot.Item == null)
        {
            return;
        }

        if (itemSlot.Item is EquippableItem)
        {
            EquippableItem currItem = (EquippableItem)itemSlot.Item;
            if (previousItemSlotData != null)
            {
                EquippableItem prevItem = (EquippableItem)previousItemSlotData.Item;
                Unequip(prevItem);
                previousItem = prevItem;
                foreach (ItemSlot _itemSlot in unitPersonalInventory.ItemSlots)
                {
                    if (_itemSlot.Item == previousItem)
                    {
                        _itemSlot.itemName.color = Color.white;
                    }
                }
            }
            if (!currItem.equipped && (previousItem == null || previousItem != currItem))
            {
                Equip(currItem);
                itemSlot.itemName.color = itemSlot.equippedColor;
            } 
            else
            {
                Unequip(currItem);
                itemSlot.itemName.color = Color.white;
            }
            unit.UpdateUI();
            unit.AssignInventory(unitPersonalInventory.GetOccupiedItemSlots());
        }
        else if (itemSlot.Item is UsableItem)
        {
            currUsableItemSlot = itemSlot;
            usableItemConfirmationPanel.SetActive(true);
            usableItemConfirmationPanel.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        previousItem = null;
    }

    public void OnUseItemButton()
    {
        usableItemConfirmationPanel.SetActive(false);
        playerInventoryPanel.SetActive(false);
        ((UsableItem)currUsableItemSlot.Item).Use(unit);
        currUsableItemSlot.Amount--;
        UpdateStatValues();
        unit.UpdateUI();
        unit.AssignInventory(unitPersonalInventory.GetOccupiedItemSlots());
        currUsableItemSlot = null;
        OnUsedUsableItem?.Invoke();
    }

    public void OnUsableItemCancelButton()
    {
        usableItemConfirmationPanel.SetActive(false);
        currUsableItemSlot = null;
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
        /*if (inventory.RemoveItem(item) && equippedItemsPanel.AddItem(item))
        {*/
        item.Equip(this.unit);
        statPanel.UpdateStatValues();
        /*}
        else
        {
            inventory.AddItem(item);
            Debug.Log("Remove items from unit to get more space");
        }*/
    }

    public void Unequip(EquippableItem item)
    {
        /*if (inventory.CanAddItem(item) && equippedItemsPanel.RemoveItem(item))
        {*/
        item.Unequip(this.unit);
        statPanel.UpdateStatValues();
            /*inventory.AddItem(item);
            equippedItemsPanel.RemoveItem(item);
        }*/
    }

    public void OnInventoryButtonClick()
    {
        unit = GameAssets.MyInstance.turnScheduler.currUnit;
        unitPersonalInventory.LoadOccupiedItemSlots(unit.unitInventory);
        Debug.Log($"panelInventory first item null = {unitPersonalInventory.ItemSlots[0].Item == null}");
        statPanel.SetStats(GetStatsToDisplay(unit));
        unitSprite.sprite = unit.closeUpImage;
        unitName.text = unit.characterName;
        UpdateStatValues();
    }

    public void UpdateStatValues()
    {
        // then call this in StatBuff item effect
        statPanel.UpdateStatValues();
    }

   /* private void AfterRightClickEvent()
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
    }*/

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
