using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Selectable Unit", menuName = "ScriptableObjects/Selectable Unit")]
public class SelectableUnit : ScriptableObject 
{
    [SerializeField]
    private string unitName;
    
    [SerializeField]
    public Sprite unitSprite;

    private SlotScript slot;

    public string UnitName
    {
        get
        {
            return unitName;
        }
    }

    public Sprite UnitSprite
    {
        get
        {
            return unitSprite;
        }
    }

    public SlotScript Slot
    {
        get
        {
            return slot;
        }
        set
        {
            slot = value;
        }
    }
}
