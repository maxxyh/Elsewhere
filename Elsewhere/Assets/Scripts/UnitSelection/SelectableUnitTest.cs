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
    public GameObject skillInfo;

    private void Start()
    {
        Debug.Log(skillInfo.activeSelf);
        skillInfo.SetActive(false);
    }
}
