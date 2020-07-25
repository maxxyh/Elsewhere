using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class ItemChest : MonoBehaviour
{
    // RMB TO ADD A COLLIDER AND A RIGIDBODY TO THE GAME OBJECT && CHECK ISTRIGGERBOX
    [SerializeField] Item item;
    [SerializeField] SpriteRenderer spriteRenderer;
    private SkeletonCommonInventory inventory;
    // Set the empty colour in inspector
    [SerializeField] Color emptyColour;
    [SerializeField] GameObject openChestPanel;
 
    private bool isInRange;
    private bool isEmpty;

    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        spriteRenderer.sprite = item.itemIcon;
        spriteRenderer.enabled = false;
        openChestPanel.SetActive(false);
    }

    private void Awake()
    {
        inventory = SkeletonCommonInventory.Instance;
    }

    private void SetPanelPosition()
    {
        // Offset position above object bbox (in world space)
        float offsetPosY = gameObject.transform.position.y + 1f;

        // Final position of marker above GO in world space
        Vector3 offsetPos = new Vector3(gameObject.transform.position.x, offsetPosY, gameObject.transform.position.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        RectTransform panelRect = openChestPanel.transform.parent.GetComponent<RectTransform>();

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, screenPoint, null, out canvasPos);

        // Set
        openChestPanel.GetComponent<RectTransform>().localPosition = canvasPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            if (!isEmpty)
            {
                SetPanelPosition();
                openChestPanel.SetActive(true);
            }
        }
    }

    public void OnOpenButton()
    {
        // Debug.Log($"Chest empty: {isEmpty}");
        if (!isEmpty)
        {
            inventory.AddItem(Instantiate(item));
            openChestPanel.SetActive(false);
            Debug.Log($"Added {item.itemName} to inventory");
            DamagePopUp.Create(gameObject.transform.position, $"{item.itemName} added to convoy", PopupType.ITEM_COLLECT);
            isEmpty = true;
        }
    }

    public void OnCancelButton()
    {
        openChestPanel.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        openChestPanel.SetActive(false);
    }
}
