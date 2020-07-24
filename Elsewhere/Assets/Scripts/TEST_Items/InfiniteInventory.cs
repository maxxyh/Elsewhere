using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteInventory : CommonInventory
{
    [SerializeField] ItemSlot itemSlotPrefab;
    [SerializeField] int maxSlots;

    public int MaxSlots
    {
        get { return maxSlots; }
        set { SetMaxSlots(value);  }
    }

    protected override void Awake()
    {
        SetMaxSlots(maxSlots);
        base.Awake();
        Debug.Log(this.ItemSlots.Count);
    }

    public override bool CanAddItem(Item item, int amount = 1)
    {
        return true;
    }

    public override bool AddItem(Item item)
    {
        while (!base.CanAddItem(item))
        {
            MaxSlots += 1;
        }
        return base.AddItem(item);
    }

    private void SetMaxSlots(int value)
    {
        base.Awake();
        if (value <= 0)
        {
            maxSlots = 1;
        }
        else
        {
            maxSlots = value;
        }

        if (maxSlots < ItemSlots.Count)
        {
            for (int i = maxSlots; i < ItemSlots.Count; i++)
            {
                Destroy(ItemSlots[i]);
            }
            int diff = ItemSlots.Count - maxSlots;
            ItemSlots.RemoveRange(maxSlots, diff);
        }
        else if (maxSlots > ItemSlots.Count)
        {
            int diff = maxSlots - ItemSlots.Count;

            for (int i = 0; i < diff; i++)
            {
                ItemSlot slot = Instantiate(itemSlotPrefab).GetComponent<ItemSlot>();
                slot.transform.SetParent(itemsParent, worldPositionStays: false);
                slot.transform.SetAsLastSibling();
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemsParent.GetComponent<RectTransform>());
                ItemSlots.Add(slot);
            }
        }
    }
}
