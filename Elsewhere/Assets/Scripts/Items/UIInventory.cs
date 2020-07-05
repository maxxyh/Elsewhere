using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    public List<UIItem> UIItems = new List<UIItem>();
    public GameObject EntryPrefab;
    public Transform inventoryPanel;

    public void UpdateSlot(int slot, Item item)
    {
        UIItems[slot].UpdateFields(item);
    }

    public void Add(Item item)
    {
        UpdateSlot(UIItems.FindIndex(i => i.item == null), item);
    }

    public void Remove(Item item)
    {
        UpdateSlot(UIItems.FindIndex(i => i.item == item), null);
    }
}