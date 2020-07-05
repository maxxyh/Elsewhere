using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TEST_ItemSlot: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text amountText;
    public Image icon;
    public Text itemName;
    public Text itemNumUses;
    [SerializeField] TEST_ItemToolTip toolTip;

    protected bool isPointerOver;

    public event Action<TEST_Item> OnRightClickEvent;

    private TEST_Item _item;
    public TEST_Item Item
    {
        get { return _item; }
        set
        {
            _item = value;

            if(_item == null && Amount != 0)
            {
                Amount = 0;
            }

            if (_item == null)
            {
                icon.enabled = false;
                itemName.enabled = false;
                itemNumUses.enabled = false;
            }
            else
            {
                icon.sprite = _item.itemIcon; 
                icon.enabled = true;
                itemName.enabled = true;
                itemNumUses.enabled = true;
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
                _amount = 0;
            }

            // use public property bc we want the setter for that property to run
            if (_amount == 0 && Item != null)
            {
                Item = null;
            }

            amountText.enabled = _item != null && _item.maxStack > 1 && _amount > 1;
            if (amountText.enabled)
            {
                amountText.text = _amount.ToString();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("On pointer click");
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null && OnRightClickEvent != null)
            {
                Debug.Log("On pointer click 2");
                OnRightClickEvent(Item);
            }
        }
    }

    public virtual void OnValidate()
    {
        if (icon == null || itemName == null || itemNumUses == null)
        {
            icon = GetComponentInChildren<Image>();
            Text[] slotTexts = GetComponentsInChildren<Text>();
            itemName = slotTexts[0];
            itemNumUses = slotTexts[1];
            // RMB TO ADD THE TEXT UI IN UNITY
            amountText = slotTexts[2];
        }
        

        if (toolTip == null)
        {
            // can use in OnValidate wo causing trouble in performance
            toolTip = FindObjectOfType<TEST_ItemToolTip>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (Item != null)
        {
            toolTip.ShowToolTip(Item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (Item != null)
        {
            toolTip.HideToolTip();
        }
    }

    protected virtual void OnDisable()
    {
        if (isPointerOver)
        {
            OnPointerEnter(null);
        }
    }
}
