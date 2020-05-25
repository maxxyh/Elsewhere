using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : Unit
{
    PlayerUnit playerStats;

     public void SelectTarget() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000)) { 
            if (hit.transform.tag == "enemy") {
                selectedUnit = hit.transform.gameObject;
                playerStats = selectedUnit.transform.gameObject.transform.GetComponent<PlayerUnit>();
            }
        }
    }

    void BasicAttack() {
        float distance = Vector3.Distance(transform.position, selectedUnit.transform.position);
        if (distance <= this.attackRange.baseValue) {
             playerStats.TakeDamage(this.attackDamage.baseValue);
        } else {
            print("Player not in range");
        }
        // https://www.youtube.com/watch?v=Hp765p29YtE
    }
    
}
