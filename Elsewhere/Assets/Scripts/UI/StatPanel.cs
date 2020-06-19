using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    [Header("Text")]
    public GameObject unitName;
    public GameObject unitClass;
    public Text unitHP;
    public Text unitMana;
    public Text unitPhysicalDamage;
    public Text unitMagicDamage;
    public Text unitArmor;
    public Text unitMagicRes;
    public Text unitMovementRange;
    public Text unitAttackRange;

    [Header("Button")]
    public Button unitHPButton;
    public Button unitManaButton;
    public Button unitPhysicalDamageButton;
    public Button unitMagicDamageButton;
    public Button unitArmorButton;
    public Button unitMagicResButton;
    public Button unitMovementRangeButton;
    public Button unitAttackRangeButton;
}
