using System;

public interface IItemContainer
{
    int ItemCount(string itemID);
    TEST_Item RemoveItem(string itemID);
    bool RemoveItem(TEST_Item item);
    // bool ContainsItem(TEST_Item item);
    
    bool AddItem(TEST_Item item);
    bool CanAddItem(TEST_Item item, int amount = 1);

    void Clear();
    // bool IsFull();
}