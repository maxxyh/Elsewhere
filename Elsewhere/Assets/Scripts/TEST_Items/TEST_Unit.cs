using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Unit : MonoBehaviour
{
    public int health = 50;

    public UnitStat physicalAttack;
    public UnitStat magicalAttack;
    public UnitStat hitRate;
    public UnitStat critRate;
    public UnitStat attackRange;

    [SerializeField] TEST_Inventory inventory;
    [SerializeField] TEST_EquippedItemsPanel equippedItemsPanel;
    [SerializeField] TEST_StatPanel statPanel;

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
        statPanel.SetStats(this.physicalAttack, this.magicalAttack, this.hitRate, this.critRate, this.attackRange);
        statPanel.UpdateStatValues();
    }
    public void Awake()
    {
        inventory.OnItemRightClickedEvent += InventoryRightClick;
        equippedItemsPanel.OnItemRightClickedEvent += EquippedItemPanelRightClick;
    }

    // item in inventory can be some other type, not nec equippable so need to check if its equippable only then will equip
    private void InventoryRightClick(TEST_Item item)
    {
        if (item is TEST_EquippableItem)
        {
            Equip((TEST_EquippableItem) item);
        }
        else if (item is TEST_UsableItem)
        {
            TEST_UsableItem usableItem = (TEST_UsableItem) item;
            usableItem.Use(this);

            if (usableItem.IsConsumable)
            {
                inventory.RemoveItem(usableItem);
                usableItem.Destroy();
            }
        }
    }
    
    private void EquippedItemPanelRightClick(TEST_Item item)
    {
        if (item is TEST_EquippableItem)
        {
            Unequip((TEST_EquippableItem) item);
        }
    }

    public void Equip(TEST_EquippableItem item)
    {
        // check if can remove item from inventory
        if (inventory.RemoveItem(item) && equippedItemsPanel.AddItem(item)) //&& equippedItemsPanel.AddItem(item))
        {
            // how to check if the panel is alrd full?
            // different from tutorial 6:35 part2
            item.Equip(this);
            statPanel.UpdateStatValues();
        }
        else
        {
            inventory.AddItem(item);
            Debug.Log("Remove items from unit to get more space");
        }
    }

    public void Unequip(TEST_EquippableItem item)
    {
        if (!inventory.IsFull() && equippedItemsPanel.RemoveItem(item))
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
}
