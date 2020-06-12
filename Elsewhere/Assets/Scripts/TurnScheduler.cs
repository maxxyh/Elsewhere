﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class TurnScheduler : StateMachine
{
    #region Fields and References
    public List<PlayerUnit> players;
    public List<EnemyUnit> enemies;
    public Queue<Unit> currTeamQueue = new Queue<Unit>();
    
    [Header("Panels")]
    public GameObject confirmationPanel;
    public GameObject playerActionPanel;
    public GameObject cancelPanel;
    public GameObject abilitiesPanel;
    public GameObject playerPhasePanel;
    public GameObject enemyPhasePanel;
    public GameObject gameOverUI;
    public GameObject victoryUI;

    public Map map;
    public Unit currUnit;

    public Team currTurn;
    private static int UnitIdCounter;

    #endregion

    #region Execution 

    public void Init(List<PlayerUnit> players, List<EnemyUnit> enemies)
    {
        this.players = players;
        this.enemies = enemies;
        UnitIdCounter = 0;
        currTurn = Team.PLAYER;
        // EnqueueTeams(Team.PLAYER);

        SetState(new Transition(this));
    }

    public void OnEndTurnButton()
    {
        StartCoroutine(State.EndTurn());
    }

    public void OnAttackButton()
    {
        StartCoroutine(State.Targeting(ActionType.ATTACK));
    }

    public void OnAbilityButton(Ability ability)
    {
        currUnit.chosenAbility = ability;
        StartCoroutine(State.Targeting(ActionType.ABILITY));
    }

    public void OnClickCheckForValidTarget(Tile tile)
    {
        StartCoroutine(State.CheckTargeting(tile));
    }

    public void StartAttack(Unit unit)
    {
        currUnit.attackingTargetUnit = unit;
        StartCoroutine(State.Attack());
    }

    public void OnYesConfirmation()
    {
        StartCoroutine(State.Yes());
    }

    public void OnNoConfirmation()
    {
        StartCoroutine(State.No());
    }

    public void OnCancelButton()
    {
        Debug.Log("cancel clicked");
        Debug.Log("Current state = " + this.State);
        StartCoroutine(State.Cancel());
    }

    public void OnAbilityMenuButton()
    {
        StartCoroutine(State.OpenMenu(MenuType.ABILITY));
    }

    public void OnExitAbilityMenuButton()
    {
        StartCoroutine(State.ReturnPreviousMenu());
    }

    public IEnumerator AttackAnimation(Unit currUnit, Unit targetUnit)
    {
        targetUnit.statPanel.SetActive(true);
        yield return StartCoroutine(currUnit.AttackAnimation());
        TraumaInducer camShakeInducer = GetComponent<TraumaInducer>();
        yield return StartCoroutine(camShakeInducer.Shake());

        // targetUnit not destroyed yet  
        if (targetUnit != null)
        {
            targetUnit.statPanel.SetActive(false);
        }
    }

    #endregion

    #region Deprecated 
    public void PlayerEndTurn()
    {
        // StartCoroutine(State.PlayerEndTurn());


        map.RemoveSelectableTiles(currUnit.currentTile);
        currUnit.EndTurn();

        currUnit.statPanel.SetActive(false);
        playerActionPanel.SetActive(false);

        // check whether there are still players in the queue -> if have then it should start the next player.
        if (currTeamQueue.Count > 0)
        {
            Transition(Team.PLAYER);
        }
        else
        {
            Transition(Team.ENEMY);
        }

    }

    // function that checks if there are still players alive on each team. If there are, it continues with the turn provided.
    public void Transition(Team turn)
    {
        // check if game has been won.
        if (players.Count == 0)
        {
            print("Battle lost. The memories are lost. Try again!");
            return;
        }
        else if (enemies.Count == 0)
        {
            print("Battle won! The memories are safe...for now.");
            return;
        }

        // there are still players alive. Check if the current queue still has players if not have to requeue.
        if (turn == Team.ENEMY)
        {
            if (currTeamQueue.Count == 0)
            {
                EnqueueTeams(Team.ENEMY);
            }
            currTurn = Team.ENEMY;
            currUnit = currTeamQueue.Dequeue();
            StartEnemyTurn();
        }
        else
        {
            if (currTeamQueue.Count == 0)
            {
                EnqueueTeams(Team.PLAYER);
            }
            currTurn = Team.PLAYER;
            currUnit = currTeamQueue.Dequeue();
            StartPlayerTurn();
        }
    }


    // currUnit.startTurn() -> update current tile + updates booleans
    // find selectable tiles 
    public void StartPlayerTurn()
    {
        currUnit.StartTurn();
        playerActionPanel.SetActive(true);
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].Value);
    }

    IEnumerator PlayerAttack()
    {
        Unit targetUnit = null;
        // wait for player to click on a valid target
        yield return new WaitUntil(() =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Unit[] units = GameObject.FindObjectsOfType<Unit>();
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("tile"))
                    {
                        Tile t = hit.collider.GetComponent<Tile>();
                        Debug.Log("hit a tile");
                        Debug.Log("occupied = " + t.occupied);
                        Debug.Log("attackable = " + t.attackable);
                        if (t.occupied && t.attackable)
                        {
                            foreach (Unit unit in units)
                            {
                                if (unit.gameObject.CompareTag("enemy") && unit.currentTile == t)
                                {
                                    Debug.Log("enemy found");
                                    targetUnit = unit;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        });
        Debug.Log("valid attack target found");
        Debug.Log("Attacking enemy. 6 damage done.");

        map.RemoveAttackableTiles();
        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(targetUnit);
        BattleManager.Battle(currUnit, targetUnit);

        yield return StartCoroutine(AttackAnimation(currUnit, targetUnit));


        PlayerEndTurn();
    }


    IEnumerator EnemyMovement()
    {
        yield return new WaitForSecondsRealtime(0.75f);

        // use distance to determine closest player
        int minDistance = int.MaxValue;
        Unit targetPlayer = players.ElementAt(0);
        foreach (Unit player in players)
        {
            AStarSearch.GeneratePath(map, currUnit.currentTile, player.currentTile, false, true);
            if (player.currentTile.distance < minDistance)
            {
                minDistance = player.currentTile.distance;
                targetPlayer = player;
            }
        }

        Tile targetTile = targetPlayer.currentTile;
        Debug.Log(targetTile.transform.position);
        AStarSearch.GeneratePath(map, currUnit.currentTile, targetPlayer.currentTile, false, true);

        // check if target tile is selectable 
        while (!targetTile.selectable)
        {
            targetTile = targetTile.parent;
        }

        /*
        // get target tile by subtracting the attackRange
        int attackRange = (int) currUnit.stats["attackRange"].Value;
        for (int i = 0; i < attackRange; i++)
        {
            if (targetTile == currUnit.currentTile)
            {
                break;
            }
            else
            {
                targetTile = targetTile.parent;
            }                
        }
        */
        // A star movement towards the target 
        currUnit.GetPathToTile(targetTile);

        yield return new WaitUntil(() => currUnit.currState == UnitState.IDLING);

        // check if there are players in range
        if (map.PlayerTargetInRange(currUnit.currentTile, currUnit.stats["attackRange"].Value, targetPlayer))
        {
            StartCoroutine(AutoEnemyAttack(targetPlayer));
        }
        else
        {
            EnemyEndTurn();
        }
    }

    public void StartEnemyTurn()
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].Value);

        StartCoroutine(EnemyMovement());


        // call it's own movement and attack functions
    }

    public void EnemyEndTurn()
    {
        map.RemoveSelectableTiles(currUnit.currentTile);
        currUnit.EndTurn();

        currUnit.statPanel.SetActive(false);

        // check whether there are still enemies in the queue -> if have then it should start the next enemies.
        if (currTeamQueue.Count > 0)
        {
            Transition(Team.ENEMY);
        }
        else
        {
            Transition(Team.PLAYER);
        }

    }

    IEnumerator AutoEnemyAttack(Unit targetPlayer)
    {
        map.RemoveSelectableTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats["attackRange"].Value);
        // should display the attacking tiles.

        yield return new WaitForSecondsRealtime(1);

        map.RemoveAttackableTiles();
        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(targetPlayer);
        BattleManager.Battle(currUnit, targetPlayer);

        yield return StartCoroutine(AttackAnimation(currUnit, targetPlayer));

        EnemyEndTurn();
    }

    #endregion

    public void EnqueueTeams(Team team = Team.BOTH) 
    {

        if (team == Team.PLAYER)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PlayerUnit unit = players.ElementAt(i);
                unit.unitID = UnitIdCounter++;
                currTeamQueue.Enqueue(unit);
            }
        }

        else if (team == Team.ENEMY)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyUnit unit = enemies.ElementAt(i);
                unit.unitID = UnitIdCounter++;
                currTeamQueue.Enqueue(unit);
            }
        }

        if (team == Team.BOTH)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PlayerUnit unit = players.ElementAt(i);
                unit.unitID = UnitIdCounter++;
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyUnit unit = enemies.ElementAt(i);
                unit.unitID = UnitIdCounter++;
            }
        }

    }

    public IEnumerator RemoveUnit(Unit deadUnit)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        deadUnit.currentTile.occupied = false;
        var toRemove = deadUnit as PlayerUnit;


        if (toRemove != null) {
            players.Remove((PlayerUnit)deadUnit);
        }
        else {
            enemies.Remove((EnemyUnit)deadUnit);
        }
        Destroy(deadUnit.gameObject);
    }
}


public enum Team 
{
    ENEMY,
    PLAYER,
    BOTH
}

public enum ActionType
{
    ATTACK,
    ABILITY
}
