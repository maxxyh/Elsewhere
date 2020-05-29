using System.Collections;
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

    PriorityQueue<Event> playerEvents = new PriorityQueue<Event>();
    PriorityQueue<Event> enemyEvents = new PriorityQueue<Event>();

    public Map map;
    private int currNoOfPlayersAlive;
    private int currNoOfEnemiesAlive;
    private Turn currTurn;
    //private Dictionary<string, List<Unit>> AliveList;
    Unit currUnit;
    private static int UnitIdCounter;

    public void InitialNo() 
    {
        currNoOfEnemiesAlive = enemies.Count;
        currNoOfPlayersAlive = players.Count;
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
        EnqueueTeams();
        // Get count of players & enemies alive
        InitialNo();
        // Start the scheduler
        //Schedule();
        StartCoroutine(ScheduleNew());
    }

    IEnumerator ScheduleNew()
    {
        while (currNoOfPlayersAlive > 0 && currNoOfEnemiesAlive > 0)
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

        if (currNoOfEnemiesAlive == 0)
        {
            print("You won!");
        }
        if (currNoOfPlayersAlive == 0)
        {
            print("You lost...");
        }
    }



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
        if (players.Remove((PlayerUnit) deadUnit))
        {
            currNoOfPlayersAlive--;
        }
        else if (enemies.Remove((EnemyUnit) deadUnit))
        {
            currNoOfEnemiesAlive--;
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

