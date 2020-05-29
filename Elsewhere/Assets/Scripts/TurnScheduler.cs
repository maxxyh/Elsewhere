using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnScheduler : MonoBehaviour
{
    private List<PlayerUnit> players;
    private List<EnemyUnit> enemies;
    PriorityQueue<Action> actionsOfPlayerTeam = new PriorityQueue<Action>();
    PriorityQueue<Action> actionsOfEnemyTeam = new PriorityQueue<Action>();
    private int currNoOfPlayersAlive;
    private int currNoOfEnemiesAlive;
    private Turn currTurn;
    //private Dictionary<string, List<Unit>> AliveList;
    Unit currUnit;
    private static int UnitIdCounter;

    public void Awake()
    {
        players = new List<PlayerUnit>(FindObjectsOfType<PlayerUnit>());
        enemies = new List<EnemyUnit>(FindObjectsOfType<EnemyUnit>());
    }

    public void InitialNo() 
    {
        currNoOfEnemiesAlive = enemies.Count;
        currNoOfPlayersAlive = players.Count;
    }

    public void Start()
    {
        
    }

    public void Init()
    {
        UnitIdCounter = 0;
        currTurn = Turn.PLAYER_TURN;
        // Enqueue start events
        EnqueueTeams();
        // Get count of players & enemies alive
        InitialNo();
        // Start the scheduler
        Schedule();
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

    public void EnqueueTeams() 
    {
        for (int i = 0; i < players.Count; i++) 
        {
            Unit unit = players[i];
            unit.unitID = UnitIdCounter++;
            actionsOfPlayerTeam.Enqueue(new StartAction(unit));
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            Unit unit = enemies[i];
            unit.unitID = UnitIdCounter++;
            actionsOfEnemyTeam.Enqueue(new StartAction(unit));
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

public enum Turn {
        ENEMY_TURN,
        PLAYER_TURN
}

