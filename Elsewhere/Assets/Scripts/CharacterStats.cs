using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterStats : MonoBehaviour
{
    // public List<BaseStat> stats = new List<BaseStat>();
    // public HealthBar healthBar;

    // void Start() 
    // {
    //     Debug.Log("PlayerStats generated");
    //     //can use a class system to determine the values of base stats
    //     stats.Add(new BaseStat(4, "Attack damage", "Physical damage."));
    //     stats.Add(new BaseStat(20, "HP", "Health points."));
    //     //Add 5 to the attack damage;
    //     stats[0].AddStatBonus(new StatBonus(5));
    //     stats[0].AddStatBonus(new StatBonus(-2));
        
    //     Debug.Log(stats[0].GetFinalStatValue());
    // }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         TakeDamage(5);
    //     }    
    // }

    // void TakeDamage(int damage) 
    // {
    //     stats[1].AddStatBonus(new StatBonus(-1 * damage));
    //     healthBar.SetHealth(stats[1].GetFinalStatValue());
    //     Debug.Log("Current health is " + stats[1].GetFinalStatValue());
    // }
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;
    // { get; private set; }

    public int damage;
    public BaseStat armor;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage) {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;
        Debug.Log(transform.name + "takes " + damage + "damage.");

        if(currentHealth <= 0) 
        {
            Die();
        }
    }

    public virtual void Die() 
    {
        //Die in sme way
        //This method is meant ot be overwritten
        Debug.Log(transform.name + " died.");
    }
}
