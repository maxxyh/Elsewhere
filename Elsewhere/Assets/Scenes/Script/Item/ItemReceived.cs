using UnityEngine;

public class ItemReceived : Interactable
{
    public Item item;

    public override void Interact() 
    {
        base.Interact();
        Receive();
    }

    void Receive() 
    {
        Debug.Log(item.itemName +  " received.");
        bool wasReceived = Inventory.instance.Add(item);
        
        if (wasReceived) {
             Destroy(gameObject);
        }
    }
}
