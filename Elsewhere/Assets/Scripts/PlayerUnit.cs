using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerUnit : Unit
{

    public Vector2 gridPosition = Vector2.zero;


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
        
        if (currState == UnitState.IDLING)
        {
            CheckMoveMouse();
        }
        
        else if (currState == UnitState.MOVING)
        {
            Move();
        }
    }

    void CheckMoveMouse()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if (t.selectable)
                    {
                        GetPathToTile(t);
                    }
                }

            }
        }
    }
}