using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableUnitTest : MonoBehaviour
{
    [SerializeField]
    public UnitInfo unitInfo;
    public bool selected;
    public Sprite unitSprite;
    public Sprite frame;
    public GameObject skillInfo;

    private void Start()
    {
        skillInfo.SetActive(false);
    }
    public void OnMouseDown()
    {
        Debug.Log("On mouse down");
        if (!selected)
        {
            selected = true;
        }
        else
        {
            selected = false;
        }
    }

    public void OnMouseEnter()
    {
        skillInfo.SetActive(true);
    }

    public void OnMouseExit()
    {
        skillInfo.SetActive(false);
    }
}
