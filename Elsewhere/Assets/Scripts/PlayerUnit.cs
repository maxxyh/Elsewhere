using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{

    public Vector2 gridPosition = Vector2.zero;


    // Update is called once per frame
    new void Update()
    {
        
        // simply update currentTile if not taking turn
        if (!takingTurn)
        {
            if (currentTile == null && map != null)
            {
                currentTile = map.GetCurrentTile(transform.position);
            }
            return;
        }

        // battling requires no input
        if (isAttacking)
        {
            return;
        }

        if (!moving && !attackingPhase)
        {
            CheckMoveMouse();
        }
        // select tile during turn within movement range and move to that tile
        else if (!moving && attackingPhase)
        {
            //CheckAttackMouse();
        }
        else 
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

    // KIV 
    void CheckAttackMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Unit[] units = GameObject.FindObjectsOfType<Unit>();
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if (t.occupied)
                    {
                        foreach(Unit unit in units)
                        {
                            if (unit.gameObject.tag == "enemy" && unit.currentTile == t)
                            {
                                StartAttack(unit);
                            }
                        }
                    }
                }

            }
        }
    }
}