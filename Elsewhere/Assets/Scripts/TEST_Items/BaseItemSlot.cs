using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Text amountText;
    [SerializeField] public Image icon;
    [SerializeField] public Text itemName;
    [SerializeField] Text itemNumUses;

    protected bool isPointerOver;

    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;
    public event Action<BaseItemSlot> OnRightClickEvent;
    public event Action<BaseItemSlot> OnLeftClickEvent;

    protected Color normalColor = Color.white;
    protected Color disabledColor = new Color(1, 1, 1, 0);
    public Color equippedColor = Color.green;

    private Item _item;
    public Item Item
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
                itemName.enabled = false;
                itemNumUses.enabled = false;
            }
            else
            {
                icon.sprite = _item.itemIcon;
                icon.color = normalColor;
                itemName.enabled = true;
                itemName.text = _item.itemName;
                itemNumUses.enabled = true;
                itemNumUses.text = _item.itemNumUses.ToString();
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

    public virtual bool CanReceiveItem(Item item)
    {
        return false;
    }

    public virtual bool CanAddStack(Item item, int amount = 1)
    {
        return Item != null && Item.ID == item.ID;
    }

    protected virtual void OnValidate()
    {
        if (icon == null)
            icon = GetComponent<Image>();

        if (amountText == null)
            amountText = GetComponentInChildren<Text>();

        if (itemName == null || itemNumUses == null)
        {
            Text[] texts = GetComponentsInChildren<Text>();
            itemName = texts[0];
            itemNumUses = texts[1];
        }

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

        if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClickEvent?.Invoke(this);
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
