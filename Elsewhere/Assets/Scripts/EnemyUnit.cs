 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : Unit
{

    // Update is called once per frame
    private void Update()
    {
        // simply update currentTile if not taking turn
        if (currState == UnitState.ENDTURN)
        {
            if (currentTile == null && map != null)
            {
                currentTile = map.GetCurrentTile(transform.position);
            }
            return;
        }

        if (currState == UnitState.MOVING)
        {
            Move();
        }
    }
 }
