﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour, IUnit
{

    #region FIELDS AND REFERENCES
    [Header("UI")]
    public GameObject statPanelGO;
    public StatPanel statPanel;
    public MajorStatPanel majorStatPanel;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();
    public Ability chosenAbility;

    [Header("Identifiers")]
    public int unitID;
    public string characterName;
    public string characterClass { get; set; }


    public Animator anim;

    [Header("Player Stats")]
    public float moveSpeed;
    public Dictionary<StatString, UnitStat> stats { get; set; }

    [Header("States")]
    private UnitState _currState = UnitState.ENDTURN;
    public UnitState CurrState
    {
        get { return _currState; }

        set { _currState = value; }
    }

    [Header("References")]
    public Unit attackingTargetUnit;
    public List<Unit> abilityTargetUnits = new List<Unit>();


    // Movement Variables
    public Map map;

    Stack<Tile> path = new Stack<Tile>();
    public Tile currentTile;
    public Tile startTile;

    Vector3 heading = new Vector3();
    Vector3 velocity = new Vector3();
    private Vector2 lastMove;

    [Header("For crystal")]
    public Transform sparkle;
    public static Action OnCrystalCollected;
    public static Action<bool> ToggleCaptureButton;
    public static Action<Unit> OnCaptureCrystal;
    private bool _onCrystal = false;
    private Crystal _crystalToCapture;
    private StatModifier _crystalBoost = new StatModifier(0.5f, StatModType.PercentAdd);

    [Header("Levelling")]
    public Level level;
    private Dictionary<StatString, int> _characterStatGrowth;
    private Dictionary<StatString, int> _classStatGrowth;

    [Header("Inventory")]
    public InBattleUnitInventoryManager unitInventory; 

    #endregion

    private void Awake()
    {
        statPanel = statPanelGO.GetComponent<StatPanel>();
        level = new Level(1, 0, OnLevelUp); // TODO make this loaded from script
        //inventory.Init(5,this);
    }

    public void OnLevelUp()
    {
        StartCoroutine(LevelUp());
    }
    private IEnumerator LevelUp()
    {
        ParticleSystem.EmissionModule tempSparkle = sparkle.GetComponent<ParticleSystem>().emission;
        tempSparkle.enabled = true;
        yield return new WaitForSeconds(0.4f);
        tempSparkle.enabled = false;

        Debug.Log("LEVEL UP");
        string increasedStats = "";

        foreach(KeyValuePair<StatString, int> entry in _characterStatGrowth)
        {
            int growthRate = entry.Value + _classStatGrowth[entry.Key];
            int toAdd = growthRate/100;
            growthRate = growthRate - toAdd * 100;
            int random = new System.Random().Next(0, 100);
            if (random <= growthRate)
                toAdd++;
            
            if (toAdd>0)
                increasedStats += $"{entry.Key} ({toAdd}), ";

            stats[entry.Key].IncreaseBaseValue(toAdd);
        }
        UpdateUI();

        Debug.Log($"Stats increased: {increasedStats}");
    }

    // Dictionary style constructor 
    public void AssignStats(Dictionary<StatString, float> input)
    {
        stats = new Dictionary<StatString, UnitStat>();
        foreach (KeyValuePair<StatString, float> pair in input)
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            this.stats[pair.Key] = new UnitStat(pair.Value, hasLimit);
        }
    }

    public void AssignStats(Dictionary<StatString, string> input)
    {
        stats = new Dictionary<StatString, UnitStat>();
        foreach (KeyValuePair<StatString, string> pair in input)
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            this.stats[pair.Key] = new UnitStat(float.Parse(pair.Value), hasLimit);
        }
    }

    public void AssignIdentity(string name, string characterClass, Dictionary<StatString, int> characterStatGrowth, Dictionary<StatString, int> classStatGrowth)
    {
        this.characterName = name;
        this.characterClass = characterClass;
        this._characterStatGrowth = characterStatGrowth;
        this._classStatGrowth = classStatGrowth;
        TextMeshProUGUI myName = statPanel.unitName.GetComponent<TextMeshProUGUI>();
        myName.SetText(name);
        TextMeshProUGUI myClass = statPanel.unitClass.GetComponent<TextMeshProUGUI>();
        myClass.SetText(characterClass);
    }


    public virtual void AssignAbilities(List<Ability> abilities)
    {
        this.abilities = abilities;
        majorStatPanel.AssignManaCost(abilities);
    }


    public void AssignMap(Map map)
    {
        this.map = map;
    }

    public void Start()
    {
        ParticleSystem.EmissionModule tempSparkle = sparkle.GetComponent<ParticleSystem>().emission;
        tempSparkle.enabled = false;

        if (CurrState == UnitState.ENDTURN)
        {
            statPanelGO.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        statPanel.UpdateStatsUI(this.stats);
        majorStatPanel.UpdateStatsUI(this.stats);
    }

    public bool isDead()
    {
        return this.stats[StatString.HP].Value <= 0;
    }

    // sets the currentTile 
    public void StartTurn()
    {
        startTile = map.GetCurrentTile(transform.position);
        currentTile = startTile;
        CurrState = UnitState.IDLING;
        statPanelGO.SetActive(true);
        CheckIfActivateCaptureButton();
    }

    public void EndTurn()
    {
        CurrState = UnitState.ENDTURN;
        this.spriteRenderer.material.SetFloat("_GrayscaleAmount", 0.75f);
        this.statPanelGO.SetActive(false);
        DecrementAllStatDuration();
        UpdateUI();
    }

    // Reset state to end turn without registering as an end turn
    public void MakeInactive()
    {
        CurrState = UnitState.ENDTURN;
        this.statPanelGO.SetActive(false);
    }

    public void RemoveGrayscale()
    {
        this.spriteRenderer.material.SetFloat("_GrayscaleAmount", 0);
    }

    public void StartAttack(Unit unit)
    {
        CurrState = UnitState.ATTACKING;
        attackingTargetUnit = unit;
    }

    public IEnumerator AttackAnimation()
    {
        AudioManager.Instance.PlayHitSound();
        anim.SetBool("isAttacking", true);
        yield return new WaitForSecondsRealtime(1f);
        anim.SetBool("isAttacking", false);
    }

    public IEnumerator AbilityAnimation()
    {
        AudioManager.Instance.PlaySpellSound();
        anim.SetBool("isAbility", true);
        yield return new WaitForSecondsRealtime(1f);
        anim.SetBool("isAbility", false);
        AudioManager.Instance.PlayHitSound();
    }

    public void TakeDamage(float damage)
    {
        stats[StatString.HP].AddModifier(new StatModifier(-damage, StatModType.Flat));
    }

    #region MOVEMENT

    // Generates the path to the tile
    public void GetPathToTile(Tile target)
    {
        AStarSearch.GeneratePath(map, currentTile, target, true);
        path.Clear();
        target.target = true;
        CurrState = UnitState.MOVING;

        Tile next = target;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    // Actually move to the tile
    public void Move()
    {
        if (path.Count > 0)
        {
            AudioManager.Instance.PlayWalkingSound();
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            // check if not reached yet - actual movement
            if (Vector3.Distance(transform.position, target) >= 0.08f)
            {
                Vector3 lastHeading = heading;
                CalculateHeading(target);
                if (heading != lastHeading)
                {
                    MovementAnimation();
                }
                SetHorizontalVelocity();

                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // Tile center reached
                transform.position = target;
                path.Pop();
                anim.SetFloat("moveSpeed", 0);
                CheckIfActivateCaptureButton();
            }
        }
        else
        {
            AudioManager.Instance.StopLoopingSFX();
            // clear currentTile and make sure it's selectable
            currentTile.Reset();
            currentTile.occupied = false;
            currentTile.selectable = true;

            CurrState = UnitState.IDLING;

            // update currentTile
            currentTile = map.GetCurrentTile(transform.position);

            //TurnManager.EndTurn();
            // TODO only EndTurn after taking an action e.g. attack, wait, defend etc.
        }
    }


    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        // make into unit vector
        heading.Normalize();
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    // only works with character thats in the scene b4 runtime
    // need to integrate with the turn manager
    public void OnMouseDown()
    {
        if (statPanelGO != null)
        {
            if (!statPanelGO.activeSelf)
            {
                statPanelGO.SetActive(true);
            }
            else
            {
                statPanelGO.SetActive(false);
            }
        }
    }

    public void MovementAnimation()
    {
        if (CurrState == UnitState.MOVING)
        {
            anim.SetFloat("moveSpeed", moveSpeed);
            anim.SetFloat("Horizontal", heading.x);
            anim.SetFloat("Vertical", heading.y);
            if (heading.x == 1 || heading.x == -1)
            {
                lastMove = new Vector2(heading.x, 0f);
            }
            else if (heading.y == 1 || heading.y == -1)
            {
                lastMove = new Vector2(0f, heading.y);
            }
            anim.SetFloat("lastMoveX", lastMove.x);
            anim.SetFloat("lastMoveY", lastMove.y);
        }
    }

    #endregion   
    public void SetStatPanelActive()
    {
        this.statPanelGO.SetActive(true);
    }

    public void SetStatPanelInActive()
    {
        if (CurrState == UnitState.ENDTURN)
        {
            this.statPanelGO.SetActive(false);
        }
    }
    
    /*protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }*/

    public void ReturnToStartTile()
    {
        currentTile.Reset();
        currentTile.occupied = false;
        transform.position = startTile.transform.position;
        currentTile = startTile;
        currentTile.occupied = true;
        lastMove.x = 0;
        lastMove.y = 0;
        anim.SetFloat("lastMoveX", lastMove.x);
        anim.SetFloat("lastMoveY", lastMove.y);
        Debug.Log("RETURN TO START TILE");
    }

    public IEnumerator SparkleAndDestroyCrystal(GameObject Crystal)
    {
        ParticleSystem.EmissionModule tempSparkle = sparkle.GetComponent<ParticleSystem>().emission;
        tempSparkle.enabled = true;
        yield return new WaitForSeconds(0.4f);
        tempSparkle.enabled = false;
        Destroy(Crystal);
    }

    public void DecrementAllStatDuration()
    {
        foreach (KeyValuePair<StatString, UnitStat> pair in stats)
        {
            pair.Value.DecrementDuration();
        }
    }

    // Manually check if need to activate capture button after start turn and moving
    private void CheckIfActivateCaptureButton()
    {
        Team currTeam = this is PlayerUnit ? Team.PLAYER : Team.ENEMY;
        if (_onCrystal )
        {
            ToggleCaptureButton(_crystalToCapture.OwnerTeam != currTeam);
        }
        else
        {
            ToggleCaptureButton(false);
        }
    }
    
    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("crystal"))
        {
            _crystalToCapture = other.GetComponent<Crystal>();
            _onCrystal = true;
        }
        
        if (other.tag == "door")
        {
            Debug.Log("Collide Door here");
            foreach(GameObject go in GameAssets.MyInstance.houseInterior)
            {
                go.SetActive(true);
            }
            other.transform.root.gameObject.SetActive(false);
        }

        yield break;
    }

    private IEnumerator OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("crystal"))
        {
            ToggleCaptureButton(false);
            _onCrystal = false;
        }

        yield break;
    }

    public void ToggleCrystalBoost(bool apply)
    {
        if (apply)
        {
            stats[StatString.MANA].AddModifier(_crystalBoost);
        }
        else
        {
            stats[StatString.MANA].RemoveModifier(_crystalBoost);
        }
    }
}


public enum UnitState
{
    IDLING,
    MOVING,
    TARGETING,
    ATTACKING,
    ENDTURN
}