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
        if (CurrState == UnitState.ENDTURN)
        {
            if (currentTile == null && map != null)
            {
                currentTile = map.GetCurrentTile(transform.position);
            }
            return;
        }

        if (CurrState == UnitState.MOVING)
        {
            Move();
        }
    }
 }
