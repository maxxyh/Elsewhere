using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// stores Items -> create Item.cs 
// can take out items 
//


public abstract class Inventory : ScriptableObject
{
    [SerializeField] public List<Item> Items;
    [SerializeField] public int Limit;
    [SerializeField] public Unit unit;
    
    public void Init(int limit, Unit unit)
    {
        this.Limit = limit;
        Items = new List<Item>();
        this.unit = unit;
    }

    public void Init(int limit, List<Item> items, Unit unit)
    {
        this.Limit = limit;
        this.Items = items;
        this.unit = unit;
    }

    public virtual void AddItem(Item item)
    {
        if (Items.Count < Limit)
        {
            Items.Add(item);
            item.inventory = this;
        }
    }

    public virtual void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

    public abstract void EquipItem(Item item);

    public abstract void UnequipItem(Item item);


    public abstract void UseItem(Item item);
}
