﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TurnScheduler : MonoBehaviour
{
    private List<PlayerUnit> players;
    private List<EnemyUnit> enemies;
    
    PriorityQueue<Action> actionsOfPlayerTeam = new PriorityQueue<Action>();
    PriorityQueue<Action> actionsOfEnemyTeam = new PriorityQueue<Action>();

    Queue<Unit> currTeamQueue = new Queue<Unit>();

    PriorityQueue<Event> playerEvents = new PriorityQueue<Event>();
    PriorityQueue<Event> enemyEvents = new PriorityQueue<Event>();

    public Map map;
    private int numPlayersAlive;
    private int numEnemiesAlive;
    private Turn currTurn;
    //private Dictionary<string, List<Unit>> AliveList;
    Unit currUnit;
    private static int UnitIdCounter;

    public void InitialNo() 
    {
        numEnemiesAlive = enemies.Count;
        numPlayersAlive = players.Count;
    }

    public void Init(List<PlayerUnit> players, List<EnemyUnit> enemies)
    {
        this.players = players;
        this.enemies = enemies;
        print("numPlayers = " + players.Count);
        print("numEnemies = " + enemies.Count);
        UnitIdCounter = 0;
        currTurn = Turn.PLAYER_TURN;
        // Enqueue start events
        // EnqueueTeams();
        EnqueueTeams("player");
        // Get count of players & enemies alive
        InitialNo();
        // Start the scheduler
        //Schedule();
        //StartCoroutine(ScheduleNew());
        NextTurn(Turn.PLAYER_TURN);
    }

    public void PlayerEndTurn()
    {
        map.RemoveSelectedTiles(currUnit.currentTile);
        currUnit.takingTurn = false;
        currUnit.isAttacking = false;
        currUnit.attackingPhase = false;

        // check whether there are still players in the queue -> if have then it should start the next player.
        if (currTeamQueue.Count > 0)
        {
            NextTurn(Turn.PLAYER_TURN);
        }
        else
        {
            NextTurn(Turn.ENEMY_TURN);
        }
       
    }

    public void EnemyEndTurn()
    {
        map.RemoveSelectedTiles(currUnit.currentTile);
        currUnit.takingTurn = false;
        currUnit.isAttacking = false;
        currUnit.attackingPhase = false;

        // check whether there are still enemies in the queue -> if have then it should start the next enemies.
        if (currTeamQueue.Count > 0)
        {
            NextTurn(Turn.ENEMY_TURN);
        }
        else
        {
            NextTurn(Turn.PLAYER_TURN);
        }

    }


    // function that checks if there are still players alive on each team. If there are, it continues with the turn provided.
    public void NextTurn(Turn turn)
    { 
        // check if game has been won.
        if (numPlayersAlive == 0)
        {
            print("Battle lost. The memories are lost. Try again!");
            return;
        }
        else if (numEnemiesAlive == 0 )
        {
            print("Battle won! The memories are safe...for now.");
            return;
        }

        // there are still players alive. Check if the current queue still has players if not have to requeue.
        if (turn == Turn.ENEMY_TURN)
        {
            if (currTeamQueue.Count == 0)
            {
                EnqueueTeams("enemy");
            }
            currTurn = Turn.ENEMY_TURN;
            currUnit = currTeamQueue.Dequeue();
            StartEnemyTurn();
        }
        else
        {
            if (currTeamQueue.Count == 0)
            {
                EnqueueTeams("player");
            }
            currTurn = Turn.PLAYER_TURN;
            currUnit = currTeamQueue.Dequeue();
            StartPlayerTurn();
        }
    }

    // currUnit.startTurn() -> update current tile + updates booleans
    // find selectable tiles 
    public void StartPlayerTurn()
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].baseValue);
    }


    // Draft 
    public void StartEnemyTurn()
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].baseValue);

        // StartCoroutine(EnemyMovement());
        
        
        // call it's own movement and attack functions
    }

    IEnumerator EnemyMovement()
    {
        yield return new WaitForSecondsRealtime(2);
        
        // use distance to determine closest player
        int minDistance = int.MaxValue;
        Unit targetPlayer = players[0];
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
        // get target tile by subtracting the attackRange
        int attackRange = (int) currUnit.stats["attackRange"].baseValue;
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
        // A star movement towards the target 
        currUnit.GetPathToTile(targetTile);
    }

    //Draft 
    public void OnEndTurnButton()
    {
        if (currTurn == Turn.PLAYER_TURN)
        {
            PlayerEndTurn();
        }
        else
        {
            EnemyEndTurn();
        }
    }

    // Draft for now 
    public void OnAttackButton()
    {
        map.RemoveSelectedTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats["attackRange"].baseValue);
        // should display the attacking tiles.

        currUnit.attackingPhase = true;

        if (currTurn == Turn.PLAYER_TURN)
        {
            Debug.Log("starting player attack");
            StartCoroutine(PlayerAttack());
        }
        else
        {
            Debug.Log("starting enemy attack");
            StartCoroutine(EnemyAttack());
        }
        
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
        
    }

    IEnumerator EnemyAttack()
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
                        if (t.occupied && t.attackable)
                        {
                            foreach (Unit unit in units)
                            {
                                if (unit.gameObject.CompareTag("player") && unit.currentTile == t)
                                {
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
        Debug.Log("Attacking player. 6 damage done.");

        map.RemoveAttackableTiles();
        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(targetUnit);
        BattleManager.Battle(currUnit, targetUnit);
        targetUnit.UpdateUI();
    }


    IEnumerator ScheduleNew()
    {
        while (numPlayersAlive > 0 && numEnemiesAlive > 0)
        {
            if (currTurn == Turn.PLAYER_TURN)
            {
                while (playerEvents.Count() > 0)
                {
                    Event currEvent = playerEvents.Dequeue();
                    currUnit = currEvent.currUnit;

                    if (currEvent.type == EventType.START)
                    {
                        currUnit.StartTurn();
                        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].baseValue);
                    }

                    else if (currEvent.type == EventType.MOVE)          
                    {
                        yield return new WaitUntil(() => !currUnit.takingTurn || currUnit.attackingPhase);
                        if (!currUnit.takingTurn)
                        {
                            Debug.Log("End Turn triggered");
                            playerEvents.Enqueue(new Event(currUnit, EventType.END));
                        }
                        else if (currUnit.attackingPhase)
                        {
                            Debug.Log("Attack Phase triggered");
                            playerEvents.Enqueue(new Event(currUnit, EventType.ATTACK));
                        }
                    }

                    else if (currEvent.type == EventType.ATTACK)
                    {
                        yield return new WaitUntil(() => currUnit.isAttacking);
                        BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);
                        playerEvents.Enqueue(new Event(currUnit, EventType.END));
                    }

                    else if (currEvent.type == EventType.END)
                    {
                        yield return new WaitUntil(() => currUnit.isAttacking);
                        map.RemoveSelectedTiles(currUnit.currentTile);
                        currUnit.isAttacking = false;
                        currUnit.attackingPhase = false;
                    }
                    Debug.Log("end player turn");
                }
                EnqueueTeams("player");
                currTurn = Turn.ENEMY_TURN;
            }

            
            else if (currTurn == Turn.ENEMY_TURN)
            {
                while (enemyEvents.Count() > 0)
                {
                    Event currEvent = enemyEvents.Dequeue();
                    currUnit = currEvent.currUnit;

                    if (currEvent.type == EventType.START)
                    {
                        currUnit.StartTurn();
                        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].baseValue);
                    }

                    else if (currEvent.type == EventType.MOVE)
                    {
                        yield return new WaitUntil(() => !currUnit.takingTurn || currUnit.attackingPhase);
                        if (!currUnit.takingTurn)
                        {
                            Debug.Log("End Turn triggered");
                            enemyEvents.Enqueue(new Event(currUnit, EventType.END));
                        }
                        else if (currUnit.attackingPhase)
                        {
                            Debug.Log("Attack Phase triggered");
                            enemyEvents.Enqueue(new Event(currUnit, EventType.ATTACK));
                        }
                    }

                    else if (currEvent.type == EventType.ATTACK)
                    {
                        yield return new WaitUntil(() => currUnit.isAttacking);
                        BattleManager.Battle(currUnit, currUnit.attackingTargetUnit);
                        enemyEvents.Enqueue(new Event(currUnit, EventType.END));
                    }

                    else if (currEvent.type == EventType.END)
                    {
                        yield return new WaitUntil(() => currUnit.isAttacking);
                        map.RemoveSelectedTiles(currUnit.currentTile);
                        currUnit.isAttacking = false;
                        currUnit.attackingPhase = false;
                    }
                    Debug.Log("end enemy turn");
                }
                EnqueueTeams("enemy");
                currTurn = Turn.PLAYER_TURN;
            }
            
        }

        if (numEnemiesAlive == 0)
        {
            print("You won!");
        }
        if (numPlayersAlive == 0)
        {
            print("You lost...");
        }
    }


    /*
    public void Schedule()
    {
        while (currNoOfPlayersAlive > 0 && currNoOfEnemiesAlive > 0)
        {
            if (currTurn == Turn.PLAYER_TURN) 
            {
                while (actionsOfPlayerTeam.Count() > 0) {
                    Action currAction = actionsOfPlayerTeam.Dequeue();
                    currUnit = currAction.currUnit;
                    Action nextAction = currAction.GenerateNextAction();
                    if (nextAction != null)
                    {
                        actionsOfPlayerTeam.Enqueue(nextAction);
                    }
                }
                foreach (PlayerUnit i in players)
                {
                    actionsOfPlayerTeam.Enqueue(new StartAction(i));
                }
                currTurn = Turn.ENEMY_TURN;
            }
            else if (currTurn == Turn.ENEMY_TURN)
            {
                 while (actionsOfEnemyTeam.Count() > 0) {
                    Action currAction = actionsOfPlayerTeam.Dequeue();
                    currUnit = currAction.currUnit;
                    Action nextAction = currAction.GenerateNextAction();
                    if (nextAction != null)
                    {
                        actionsOfEnemyTeam.Enqueue(nextAction);
                    }
                }
                foreach (EnemyUnit i in enemies)
                {
                    actionsOfEnemyTeam.Enqueue(new StartAction(i));
                }
                currTurn = Turn.PLAYER_TURN;
            }
        } 

        if (currNoOfEnemiesAlive == 0) 
        {
            print("You won!");
        }
        if (currNoOfPlayersAlive == 0)
        {
            print("You lost...");
        }
    }
    */
    public void EnqueueTeams(string team = "both") 
    {

        if (team == "player")
        {
            for (int i = 0; i < players.Count; i++)
            {
                PlayerUnit unit = players[i];
                unit.unitID = UnitIdCounter++;
                actionsOfPlayerTeam.Enqueue(new StartAction(unit));
                playerEvents.Enqueue(new Event(unit, EventType.START));
                currTeamQueue.Enqueue(unit);
            }
        }

        else if (team == "enemy")
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyUnit unit = enemies[i];
                unit.unitID = UnitIdCounter++;
                actionsOfEnemyTeam.Enqueue(new StartAction(unit));
                enemyEvents.Enqueue(new Event(unit, EventType.START));
                currTeamQueue.Enqueue(unit);
            }
        }

        if (team == "both")
        {
            for (int i = 0; i < players.Count; i++)
            {
                PlayerUnit unit = players[i];
                unit.unitID = UnitIdCounter++;
                actionsOfPlayerTeam.Enqueue(new StartAction(unit));
                playerEvents.Enqueue(new Event(unit, EventType.START));
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyUnit unit = enemies[i];
                unit.unitID = UnitIdCounter++;
                actionsOfEnemyTeam.Enqueue(new StartAction(unit));
                enemyEvents.Enqueue(new Event(unit, EventType.START));
            }
        }
    }

    public void currentPlayerAttack()
    {
        currUnit.attackingPhase = true;
    }

    public void currentPlayerEndTurn()
    {
        currUnit.EndTurn();
    }

    public void RemoveUnit(Unit deadUnit)
    {
        deadUnit.currentTile.occupied = false;
        var toRemove = deadUnit as PlayerUnit;

        if (toRemove != null) {
            players.Remove((PlayerUnit)deadUnit);
            numPlayersAlive--;
        }
        else {
            enemies.Remove((EnemyUnit)deadUnit);
            numEnemiesAlive--;
        }
        Destroy(deadUnit.gameObject);
    }
}

public enum Turn 
{
    ENEMY_TURN,
    PLAYER_TURN
}

public enum EventType
{
    START,
    MOVE,
    ATTACK,
    END
}

