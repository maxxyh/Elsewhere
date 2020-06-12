using System.Collections;
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
    public LinkedList<Unit> currTeamQueue = new LinkedList<Unit>();
    
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

    public IEnumerator AbilityAnimation(Unit currUnit)
    { 
        yield return StartCoroutine(currUnit.AbilityAnimation());
        TraumaInducer camShakeInducer = GetComponent<TraumaInducer>();
        yield return StartCoroutine(camShakeInducer.Shake());
    }

    #endregion

    #region Deprecated 

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


        //PlayerEndTurn();
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
                currTeamQueue.AddLast(unit);
            }
        }

        else if (team == Team.ENEMY)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyUnit unit = enemies.ElementAt(i);
                unit.unitID = UnitIdCounter++;
                currTeamQueue.AddLast(unit);
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
