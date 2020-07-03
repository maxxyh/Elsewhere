using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public Item item;
    public Image spriteImage;
    public Text itemName;
    public Text numUses;

    private void Awake()
    {
        UpdateFields(null);
    }

    public void UpdateFields()
    {
        spriteImage.sprite = item.icon;
        itemName.text = item.itemName;
        numUses.text = item.numUses.ToString();
    }
    
    public void UpdateFields(Item item)
    {
        this.item = item;
        if (this.item != null)
        {
            spriteImage.sprite = item.icon;
            itemName.text = item.itemName;
            numUses.text = item.numUses.ToString();
        } else
        {
            spriteImage.color = Color.clear;
            itemName.text = "";
            numUses.text = "";
        }
    }
}