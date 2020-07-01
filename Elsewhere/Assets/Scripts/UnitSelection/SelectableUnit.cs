using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Selectable Unit", menuName = "ScriptableObjects/Selectable Unit")]
public class SelectableUnit : ScriptableObject 
{
    public string unitName;
    public Sprite unitSprite;
}
