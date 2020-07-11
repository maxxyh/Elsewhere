using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] Text itemNameText;
    [SerializeField] Text itemTypeText;
    [SerializeField] Text itemDescriptionText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowToolTip(Item item)
    {
        itemNameText.text = item.itemName;
        itemTypeText.text = item.GetItemType();
        itemDescriptionText.text = item.GetDescription();
        
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
        
}
