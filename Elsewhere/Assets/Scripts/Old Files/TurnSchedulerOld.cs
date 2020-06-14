using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class TurnSchedulerOld : MonoBehaviour
{
    private List<PlayerUnit> players;
    private List<EnemyUnit> enemies;
    
    PriorityQueue<Action> actionsOfPlayerTeam = new PriorityQueue<Action>();
    PriorityQueue<Action> actionsOfEnemyTeam = new PriorityQueue<Action>();

    Queue<Unit> currTeamQueue = new Queue<Unit>();

    public GameObject playerPanel;

    PriorityQueue<Event> playerEvents = new PriorityQueue<Event>();
    PriorityQueue<Event> enemyEvents = new PriorityQueue<Event>();

    public Map map;
    private int numPlayersAlive;
    private int numEnemiesAlive;
    private Team currTurn;
    //private Dictionary<string, List<Unit>> AliveList;
    public Unit currUnit;
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
        currTurn = Team.PLAYER;
        // Enqueue start events
        // EnqueueTeams();
        EnqueueTeams("player");
        // Get count of players & enemies alive
        InitialNo();
        // Start the scheduler
        //Schedule();
        //StartCoroutine(ScheduleNew());
        NextTurn(Team.PLAYER);
    }

    public void PlayerEndTurn()
    {
        map.RemoveSelectableTiles(currUnit.currentTile);
        currUnit.EndTurn();

        currUnit.statPanel.SetActive(false);
        playerPanel.SetActive(false);

        // check whether there are still players in the queue -> if have then it should start the next player.
        if (currTeamQueue.Count > 0)
        {
            NextTurn(Team.PLAYER);
        }
        else
        {
            NextTurn(Team.ENEMY);
        }
       
    }

    public void EnemyEndTurn()
    {
        map.RemoveSelectableTiles(currUnit.currentTile);
        currUnit.EndTurn();

        currUnit.statPanel.SetActive(false);

        // check whether there are still enemies in the queue -> if have then it should start the next enemies.
        if (currTeamQueue.Count > 0)
        {
            NextTurn(Team.ENEMY);
        }
        else
        {
            NextTurn(Team.PLAYER);
        }

    }


    // function that checks if there are still players alive on each team. If there are, it continues with the turn provided.
    public void NextTurn(Team turn)
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
        if (turn == Team.ENEMY)
        {
            if (currTeamQueue.Count == 0)
            {
                EnqueueTeams("enemy");
            }
            currTurn = Team.ENEMY;
            currUnit = currTeamQueue.Dequeue();
            StartEnemyTurn();
        }
        else
        {
            if (currTeamQueue.Count == 0)
            {
                EnqueueTeams("player");
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
        playerPanel.SetActive(true);
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].baseValue);
    }


    // Draft 
    public void StartEnemyTurn()
    {
        currUnit.StartTurn();
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].baseValue);

        StartCoroutine(EnemyMovement());
        
        
        // call it's own movement and attack functions
    }

    IEnumerator EnemyMovement()
    {
        yield return new WaitForSecondsRealtime(0.75f);
        
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
        
        // check if target tile is selectable 
        while (!targetTile.selectable)
        {
            targetTile = targetTile.parent;
        }

        /*
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
        */
        // A star movement towards the target 
        currUnit.GetPathToTile(targetTile);

        yield return new WaitUntil(() => currUnit.currState == UnitState.IDLING);

        // check if there are players in range
        if (map.PlayerTargetInRange(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].baseValue, targetPlayer))
        {
            StartCoroutine(AutoEnemyAttack(targetPlayer));
        }
        else
        {
            EnemyEndTurn();
        }
    }

    //Draft 
    public void OnEndTurnButton()
    {
        if (currTurn == Team.PLAYER)
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
        map.RemoveSelectableTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].baseValue);
        // should display the attacking tiles.

        currUnit.currState = UnitState.TARGETING;

        if (currTurn == Team.PLAYER)
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

    IEnumerator AttackAnimation(Unit targetUnit)
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

        yield return StartCoroutine(AttackAnimation(targetUnit));
        

        PlayerEndTurn();
    }

    IEnumerator AutoEnemyAttack(Unit targetPlayer)
    {
        map.RemoveSelectableTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].baseValue);
        // should display the attacking tiles.

        yield return new WaitForSecondsRealtime(1);

        map.RemoveAttackableTiles();
        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(targetPlayer);
        BattleManager.Battle(currUnit, targetPlayer);

        yield return StartCoroutine(AttackAnimation(targetPlayer));

        EnemyEndTurn();
    }

    // out of use.
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
        TraumaInducer camShakeInducer = GetComponent<TraumaInducer>();
        StartCoroutine(camShakeInducer.Shake());
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

    public IEnumerator RemoveUnit(Unit deadUnit)
    {
        yield return new WaitForSecondsRealtime(0.3f);
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


/*
public enum Turn 
{
    ENEMY_TURN,
    PLAYER_TURN
}
*/

public enum EventType
{
    START,
    MOVE,
    ATTACK,
    END
}

