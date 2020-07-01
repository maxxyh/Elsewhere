using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject[] selectableUnitButtons;

    private void Start()
    {
        foreach(GameObject button in selectableUnitButtons)
        {
            GameObject charCell = Instantiate(button, transform);
        }
    }

    
}
