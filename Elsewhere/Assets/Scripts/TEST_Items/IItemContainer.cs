using System;

public interface IItemContainer
{
    int ItemCount(string itemID);
    Item RemoveItem(string itemID);
    bool RemoveItem(Item item);
    // bool ContainsItem(TEST_Item item);
    
    bool AddItem(Item item);
    bool CanAddItem(Item item, int amount = 1);

    void Clear();
    // bool IsFull();
}