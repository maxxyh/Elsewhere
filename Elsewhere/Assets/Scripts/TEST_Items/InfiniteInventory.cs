using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void SetMaxSlots(int value)
    {
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
                gameObject.transform.SetParent(itemsParent, worldPositionStays: false);
                ItemSlots.Add(slot);
            }
        }
    }
}
