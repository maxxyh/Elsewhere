using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class ItemChest : MonoBehaviour
{
    // RMB TO ADD A COLLIDER AND A RIGIDBODY TO THE GAME OBJECT && CHECK ISTRIGGERBOX
    [SerializeField] Item item;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] CommonInventory inventory;
    // Set the empty colour in inspector
    [SerializeField] Color emptyColour;
    [SerializeField] KeyCode itemPickupKeyCode = KeyCode.E;
    [SerializeField] GameObject openChestPanel;
    
    private bool isInRange;
    private bool isEmpty;

    private void OnValidate()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<CommonInventory>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        spriteRenderer.sprite = item.itemIcon;
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(itemPickupKeyCode))
        {
            if(!isEmpty)
            {
                inventory.AddItem(Instantiate(item));
                isEmpty = true;
                spriteRenderer.color = emptyColour;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckCollision(collision.gameObject, false);
    }

    private void CheckCollision(GameObject go, bool state)
    {
        if (gameObject.CompareTag("player"))
        {
            isInRange = state;
            spriteRenderer.enabled = state;
        }
    }
}
