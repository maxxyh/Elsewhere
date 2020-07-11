using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableUnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject selectableUnitGO;

    [SerializeField]
    private Button buttonWithImage;

    private GameObject skillPanel;
    private bool selected = false;
    private Text unitName;

    private void Awake()
    {
        buttonWithImage.image.sprite = selectableUnitGO.GetComponent<SelectableUnitTest>().unitSprite;
        skillPanel = selectableUnitGO.GetComponent<SelectableUnitTest>().skillInfo;
        unitName = this.GetComponentInChildren<Text>();
    }
    private void Start()
    {
        unitName.text = selectableUnitGO.GetComponent<SelectableUnitTest>().unitInfo.unitID;
        buttonWithImage.onClick.AddListener(delegate { Select(); });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        skillPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillPanel.SetActive(false);
    }

    public void Select()
    {
        Debug.Log("On mouse click");
        if (!selected)
        {
            selected = true;
        }
        else
        {
            selected = false;
        }
        selectableUnitGO.GetComponent<SelectableUnitTest>().selected = selected;
    }
}
