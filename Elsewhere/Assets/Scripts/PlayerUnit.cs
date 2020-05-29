using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{

    public Vector2 gridPosition = Vector2.zero;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        
        // simply update currentTile if not taking turn
        if (!takingTurn)
        {
            currentTile = map.GetCurrentTile(Vector3Int.RoundToInt(transform.position));
            return;
        }

        // battling requires no input
        if (isAttacking)
        {
            return;
        }

        if (!moving && !attackingPhase)
        {
            // map.FindSelectableTiles(currentTile,this.stats["movementRange"].baseValue);
            CheckMoveMouse();
        }
        // select tile during turn within movement range and move to that tile
        else if (!moving && attackingPhase)
        {
            CheckAttackMouse();
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