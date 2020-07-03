using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IEquippable
{ 
    void Equip();

    void UnEquip();

    void UseWeapon();

    Dictionary<StatString, int> GetEquipBonus();

}
