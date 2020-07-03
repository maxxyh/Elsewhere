using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] public int numUses;
    [HideInInspector] public Inventory inventory;

}
