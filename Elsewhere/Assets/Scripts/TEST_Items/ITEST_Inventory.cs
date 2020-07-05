using System;

public interface ITEST_Inventory
{
    int ItemCount(string itemID);
    TEST_Item RemoveItem(string itemID);
    bool ContainsItem(Item item);
    bool AddItem(TEST_Item item);
    bool IsFull();
    bool RemoveItem(TEST_Item item);
}