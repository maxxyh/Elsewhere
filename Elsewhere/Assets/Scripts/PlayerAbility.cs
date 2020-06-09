using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class PlayerAbility : State
{
    public PlayerAbility(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        Ability ability = turnScheduler.currUnit.chosenAbility;

        Team targetTeam;
        if (turnScheduler.currTurn == Team.ENEMY)
        {
            if (ability.targetsSameTeam)
            {
                targetTeam = Team.ENEMY;
            }
            else
            {
                targetTeam = Team.PLAYER;
            }
        }
        else
        {
            if (ability.targetsSameTeam)
            {
                targetTeam = Team.PLAYER;
            }
            else
            {
                targetTeam = Team.ENEMY;
            }
        }


        map.RemoveSelectedTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, ability.attackRange);
        // should display the attacking tiles.
        
        currUnit.currState = UnitState.TARGETING;
        Debug.Log("starting player attack");


        // now I need to find my targets 

        // two styles: 1 is the traditional one, 2 is the click and will display the effect...
        // But how to do the undo button?? use the waitUntil.
        
        List<Unit> targetUnits = new List<Unit>();
        
        // Traditional style
        if (ability.isSingleTarget)
        {
            Unit targetUnit = null;

            // wait for player to click on a valid target
            yield return new WaitUntil(() =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("tile"))
                        {
                            Tile t = hit.collider.GetComponent<Tile>();
                            
                            if (targetTeam == Team.ENEMY)
                            {
                                foreach (Unit unit in turnScheduler.enemies)
                                {
                                    if (unit.currentTile == t)
                                    {
                                        targetUnit = unit;
                                        targetUnits.Add(targetUnit);

                                        turnScheduler.confirmationPanel.SetActive(true);

                                        // Unity Events would be a good approach here, but now just using a dirty approach.
                                        if (turnScheduler.confirmationPanel.GetComponent<ConfirmationPanelScript>().IsConfirmed())
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                }
                            } 
                            else
                            {
                                foreach (Unit unit in turnScheduler.players)
                                {
                                    if (unit.currentTile == t)
                                    {
                                        targetUnit = unit;
                                        targetUnits.Add(targetUnit);

                                        turnScheduler.confirmationPanel.SetActive(true);

                                        // Unity Events would be a good approach here, but now just using a dirty approach.
                                        if (turnScheduler.confirmationPanel.GetComponent<ConfirmationPanelScript>(). IsConfirmed())
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                                
                            
                        }
                    }
                }
                return false;
            });
        }
        else
        {
            // multi-targeting??
        }

        map.RemoveAttackableTiles();    

        yield return turnScheduler.StartCoroutine(ability.Execute(targetUnits));
        
        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));

        yield break;
    }
}
