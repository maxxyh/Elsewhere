using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : Unit
{
    private void OnDrawGizmosSelected()
    {
        if (this == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(this.transform.position, 0.8f);
    }

    EnemyUnit enemyStats;
    public void SelectTarget() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null) {
                Debug.Log(hit.collider.gameObject.tag);
                if (hit.collider.tag == "enemy") {
                    selectedUnit = hit.collider.gameObject;
                    enemyStats = selectedUnit.transform.gameObject.transform.GetComponent<EnemyUnit>();
                }
            }
        }

        
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics2D.Raycast(ray, out hit, 10000)) { 
        //     if (hit.transform.tag == "enemy") {
        //         selectedUnit = hit.transform.gameObject;
        //         enemyStats = selectedUnit.transform.gameObject.transform.GetComponent<EnemyUnit>();
        //     }
        // }
//         RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
 
// if(hit.collider != null)
// {
//     Debug.Log ("Target Position: " + hit.collider.gameObject.transform.position);
// }
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
