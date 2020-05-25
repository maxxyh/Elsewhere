using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public GameObject owner;

    [SerializeField]
    private float magnitude = 3;

    [SerializeField] 
    private float manaCost = 3;
    
    // public void hit(GameObject target) 
    // {
    //     // find distance between target and owner
    //     float distance = Vector3.Distance(target.transform.position, owner.transform.position);
       
    //     Character ownerStats = this.owner.GetComponent<Character>();
    //     Character targetStats = target.GetComponent<Character>();
    //     // Physics.OverlapSphere: it returns an array of all colliders that 
    //     // intersect an imaginary sphere of a given radius. You can then 
    //     // check the tags to know which ones are enemies, and apply the damage:
    //     Collider[] cols = Physics.OverlapSphere(transform.position, distance);
    //     foreach (Collider col in cols)
    //     {
    //          if (col && col.tag == "Player") {
    //     }
        
    // {

    //     if (ownerStats.mana.baseValue >= this.manaCost) 
    //     {
    //         targetStats.TakeDamage(magnitude);
    //         ownerStats.mana.baseValue -= this.manaCost;
    //     }
    // }

    public void Update() {

    }
}