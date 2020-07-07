using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TEST_BaseItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Text amountText;
    [SerializeField] public Image icon;
    /*public Text itemName;
    public Text itemNumUses;*/
    // [SerializeField] TEST_ItemToolTip toolTip;

    protected bool isPointerOver;

    // public event Action<TEST_Item> OnRightClickEvent;

    public event Action<TEST_BaseItemSlot> OnPointerEnterEvent;
    public event Action<TEST_BaseItemSlot> OnPointerExitEvent;
    public event Action<TEST_BaseItemSlot> OnRightClickEvent;

    protected Color normalColor = Color.white;
    protected Color disabledColor = new Color(1, 1, 1, 0);

    private TEST_Item _item;
    public TEST_Item Item
    {
        get { return _item; }
        set
        {
            _item = value;

            if (_item == null && Amount != 0)
            {
                Amount = 0;
            }

            if (_item == null)
            {
                icon.sprite = null;
                icon.color = disabledColor;
            }
            else
            {
                icon.sprite = _item.itemIcon;
                icon.color = normalColor;
            }

            if (isPointerOver)
            {
                // fixes bug for panel still appears when the item is alrd clicked and changed position by simulating the enter and exit event
                OnPointerExit(null);
                OnPointerEnter(null);
            }
        }
    }

    private int _amount;
    public int Amount
    {
        get
        {
            return _amount;
        }
        set
        {
            _amount = value;
            if (_amount < 0)
            {
                Item = null;
                _amount = 0;
            }

            // use public property bc we want the setter for that property to run
            if (_amount == 0 && Item != null)
            {
                Item = null;
            }
            if (amountText != null)
            {
                amountText.enabled = _item != null && _amount > 1;
                if (amountText.enabled)
                {
                    amountText.text = _amount.ToString();
                }
            }
        }
    }

    public virtual bool CanReceiveItem(TEST_Item item)
    {
        return false;
    }

    public virtual bool CanAddStack(TEST_Item item, int amount = 1)
    {
        /*if (Item == null)
        {
            Debug.Log("Item in base slot null");
        }
        if (item == null)
        {
            Debug.Log("item in base slot null");
        }
        if (Item.ID == null)
        {
            Debug.Log("ItemID in base slot null");
        }
        if (item.ID == null)
        {
            Debug.Log("itemID in base slot null");
        }*/
        return Item != null && Item.ID == item.ID;
    }

    protected virtual void OnValidate()
    {
        if (icon == null)
            icon = GetComponent<Image>();

        if (amountText == null)
            amountText = GetComponentInChildren<Text>();

        Item = _item;
        Amount = _amount;
    }

    protected virtual void OnDisable()
    {
        if (isPointerOver)
        {
            OnPointerExit(null);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClickEvent?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        OnPointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        OnPointerExitEvent?.Invoke(this);
    }
}
