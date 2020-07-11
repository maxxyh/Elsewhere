using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject tutorialPanel;

    [Header("Dialogue")]
    public DialogueDisplay openingDialogue;
    public GameObject startPlayerTurnDialogue;
    public GameObject moveDiaglogue;
    public GameObject attackDialogue;
    public GameObject nextChooseDialogue;
    public GameObject abilityClickDialogue;
    public GameObject abilityChoosingDialogue;
    public GameObject abilityExecutingDialogue;
    public GameObject startEnemyTurnDialogue;

    public Map map;
    public Unit currUnit;

    public Team currTurn;
    private static int UnitIdCounter;
    public int TutTurn = 0;

    [SerializeField] public ObjectiveType objectiveType;
    public bool objectiveCompleted = false;

    [SerializeField] private bool skipTutorial;

    [SerializeField] private TraumaInducer camShakeInducer;

    #endregion

    #region Execution 

    private void Awake()
    {
        camShakeInducer = GetComponent<TraumaInducer>();
        PanelManager.OnCrystalCaptureCutSceneDone += OnCrystalCaptureCutSceneDone;
        Crystal.ReturnControlToState += OnReturnControlToState;
    }

    public void Init(List<PlayerUnit> players, List<EnemyUnit> enemies)
    {
        this.players = players;
        this.enemies = enemies;
        UnitIdCounter = 0;
        currTurn = Team.PLAYER;
        if (SceneManager.GetActiveScene().name.Equals("Tutorial") && !skipTutorial)
        {
            currTurn = Team.PLAYER;
            SetState(new CutScene(this));
        }
        else
        {
            currTurn = Team.ENEMY;
            SetState(new Transition(this));
        }        
    }

    public void OnCrystalCaptureCutSceneDone(bool allCrystalsCollected)
    {
        if (allCrystalsCollected && objectiveType == ObjectiveType.COLLECT_ALL_CRYSTALS)
        {
            objectiveCompleted = true;
        }

        StartCoroutine(State.CrystalCaptureCutSceneDone());
    }

    public void OnReturnControlToState()
    {
        StartCoroutine(State.CrystalCaptureCutSceneDone());
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

    public void DuringCutScene()
    {
        StartCoroutine(State.DuringCutScene());
    }

    public void WaitForDialogueEnd()
    {
        StartCoroutine(State.DuringDialogue());
    }

    public void OnCancelButton()
    {
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

    public void OnCaptureButton()
    {
        StartCoroutine(State.Capture());
    }

    public IEnumerator AttackAnimation(Unit currUnit, Unit targetUnit)
    {
        targetUnit.statPanelGO.SetActive(true);
        StartCoroutine(camShakeInducer.Shake());
        yield return StartCoroutine(currUnit.AttackAnimation());

        // targetUnit not destroyed yet  
        if (targetUnit != null)
        {
            targetUnit.statPanelGO.SetActive(false);
        }
    }

    public IEnumerator AbilityAnimation(Unit currUnit)
    { 
        yield return StartCoroutine(currUnit.AbilityAnimation());
        StartCoroutine(camShakeInducer.Shake());
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

    private void OnDestroy()
    {
        if (PanelManager.OnCrystalCaptureCutSceneDone != null)
            PanelManager.OnCrystalCaptureCutSceneDone -= OnCrystalCaptureCutSceneDone;
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


public enum ObjectiveType
{
    ELIMINATE_ALL_ENEMIES,
    COLLECT_ALL_CRYSTALS
}