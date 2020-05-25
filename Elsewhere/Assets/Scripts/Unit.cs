using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
     public GameObject Panel;
    public string characterName;
    public UnitStat attackDamage;
    public UnitStat magicDamage;
    public UnitStat mana;
    public UnitStat HP;
    public UnitStat defence;
    public UnitStat movementRange;
    public UnitStat attackRange;
    public bool isDead = false;
    public float currHP;
    public GameObject selectedUnit;
    
    public void Start()
    {
        currHP = this.HP.CalculateFinalValue();
    }

    public void Update()
    {
        if (currHP <= 0) {
            isDead = true;
            currHP = 0;
        }
    }

    public void TakeDamage(float damage) {
        currHP -= damage;
        print ("damage done: " +  damage);
        print ("my HP: " + currHP);
        if (this.HP.baseValue <= 0) {
            isDead = true;
            this.gameObject.tag = "DeadPlayerUnit";
            Destroy(this.gameObject);
        }
    }
}
