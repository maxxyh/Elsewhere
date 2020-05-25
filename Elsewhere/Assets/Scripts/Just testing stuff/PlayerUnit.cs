using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : Unit
{
    EnemyUnit enemyStats;
    public void SelectTarget() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000)) { 
            if (hit.transform.tag == "enemy") {
                selectedUnit = hit.transform.gameObject;
                enemyStats = selectedUnit.transform.gameObject.transform.GetComponent<EnemyUnit>();
            }
        }
    }

    void BasicAttack() {
        float distance = Vector3.Distance(transform.position, selectedUnit.transform.position);
        if (distance <= this.attackRange.baseValue) {
             enemyStats.TakeDamage(this.attackDamage.baseValue);
        } else {
            print("Enemy not in range");
        }
        // https://www.youtube.com/watch?v=Hp765p29YtE
    }

    public void OnMouseDown() 
    {
        if (Panel != null) {
            if (!Panel.activeSelf) {
                Panel.SetActive(true);
                print(Panel.activeSelf);
            } else {
                Panel.SetActive(false);
                print(Panel.activeSelf);
            }
        }    
    }
}
