using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private int _stunDuration = 0;
    [SerializeField] private ParticleSystem stunEffect;
    public bool isStunned => _stunDuration > 0;
    public List<Unit> abilityTargetUnits = new List<Unit>();
    public Tile abilityTargetTile;
    
    public Unit attackingTargetUnit;

    [Header("Identifiers")]    public int unitID;
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
        get => _currState;

        set => _currState = value;
    }


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
    // public InBattleUnitInventoryManager unitInventoryManager; 
    [HideInInspector] public List<ItemSlotData> unitInventory = new List<ItemSlotData>();

    [HideInInspector] public int equippedWeaponIndex = -1; 
    private const int UnitInventorySize = 3;

    [FormerlySerializedAs("unitSprite")] public Sprite closeUpImage;

    #endregion

    private void Awake()
    {
        statPanel = statPanelGO.GetComponent<StatPanel>();
        //inventory.Init(5,this);
    }

    public void OnLevelUp()
    {
        StartCoroutine(LevelUp());
        DamagePopUp.Create(this.transform.position, "LEVEL UP", PopupType.LEVEL_UP);
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

    public void CreateUnit(UnitLoadData unitLoadData, JObject abilityConfig, 
        Dictionary<StatString, int> classStatGrowth, Dictionary<StatString, int> characterStatGrowth)
    {
        AssignStats(unitLoadData.unitStats);
        AssignLevel(unitLoadData.unitLevel, unitLoadData.unitExp);
        AssignAbilities(unitLoadData.unitAbilities, abilityConfig);
        AssignIdentity(unitLoadData.unitName, unitLoadData.unitClass, characterStatGrowth, classStatGrowth);
        AssignInventory(unitLoadData.unitInventory);
    }

    private void AssignLevel(int currLevel, int currExp)
    {
        level = new Level(currLevel, currExp, OnLevelUp);
    }
    
    private void AssignStats(Dictionary<StatString, int> input)
    {
        stats = new Dictionary<StatString, UnitStat>();
        foreach (KeyValuePair<StatString, int> pair in input)
        {
            bool hasLimit = pair.Key.Equals(StatString.HP) || pair.Key.Equals(StatString.MANA);
            this.stats[pair.Key] = new UnitStat(pair.Value, hasLimit);
        }
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

    public void AssignInventory(List<Item> items)
    {
        unitInventory.Clear();
        for (int i = 0; i < items.Count && i < UnitInventorySize ; i++)
        {
            Item item = items[i];
            ItemSlotData match = unitInventory.Find(x => x.Item == item);
            if (match != null)
            {
                unitInventory.Find(x => x.Item == item).Amount++;
            }
            else
            {
                unitInventory.Add(new ItemSlotData(item, 1));
            }
        }

        equippedWeaponIndex = FindEquippedItemIndex(unitInventory);
    }

    public void AssignInventory(List<ItemSlot> itemSlots)
    {
        unitInventory.Clear();
        foreach (ItemSlot itemSlot in itemSlots)
        {
            unitInventory.Add(new ItemSlotData(itemSlot.Item, itemSlot.Amount));
        }

        equippedWeaponIndex = FindEquippedItemIndex(unitInventory);
    }

    private static int FindEquippedItemIndex(List<ItemSlotData> unitInventory)
    {
        ItemSlotData equippedItemSlotData = unitInventory.Find(x =>
        {
            if (x.Item is EquippableItem)
            {
                return ((EquippableItem) x.Item).equipped;
            }

            return false;
        });
        if (equippedItemSlotData != null)
        {
            return unitInventory.FindIndex(x => x == equippedItemSlotData);
        }
        else
            return -1;
    }
    
        
    public virtual void AssignAbilities(List<Ability> abilities)
    {
        this.abilities = abilities;
        majorStatPanel.AssignManaCost(abilities);
    }

    public virtual void AssignAbilities(IEnumerable<string> abilityNames, JObject abilityConfig)
    {
        abilities = new List<Ability>();
        foreach (string abilityName in abilityNames)
        {
            abilities.Add(StaticData.AbilityReference[abilityName]);
            majorStatPanel.AddAbilityToPanel((string) abilityConfig[abilityName]["name"], 
                (string) abilityConfig[abilityName]["description"], StaticData.AbilityReference[abilityName].GetManaCost());
        }
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
        statPanel.UpdateStatsUI(this.stats, this.level);
        majorStatPanel.UpdateStatsUI(this.stats, this.level);
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
        LookFront();
        DecrementAllStatDuration();
        DecrementStun();
        UpdateUI();
    }

    private void LookFront()
    {
        lastMove.x = 0;
        lastMove.y = -1;
        anim.SetFloat("lastMoveX", lastMove.x);
        anim.SetFloat("lastMoveY", lastMove.y);
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

    public void UseWeapon()
    {
        int index = FindEquippedItemIndex(unitInventory);
        if (index != -1)
        {
            Debug.Log($"numUses before = {unitInventory[index].Item.itemNumUses}");
            unitInventory[index].Item.itemNumUses--;
            Debug.Log($"numUses after = {unitInventory[index].Item.itemNumUses}");
        }
        
        if (unitInventory[index].Item.itemNumUses == 0)
        {
            if (unitInventory[index].Item is EquippableItem)
            {
                EquippableItem equippableItem = (EquippableItem) unitInventory[index].Item;
                equippableItem.Unequip(this);
            }
        }
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
            Debug.Log("Collide Crystal here");
            _crystalToCapture = other.GetComponent<Crystal>();
            // _crystalToCapture.crystalBubble.SetActive(true);
            _onCrystal = true;
        }

        if (other.CompareTag("door") && this is PlayerUnit)
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
            _crystalToCapture.crystalBubble.SetActive(false);
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

    public void Stun()
    {
        _stunDuration = 2;
    }

    private void DecrementStun()
    {
        _stunDuration--;
    }

    public IEnumerator StunAnimation()
    {
        Debug.Log("Stun animation");
        stunEffect.Play();
        DamagePopUp.Create(transform.position, "Stunned!", PopupType.DAMAGE);
        yield return new WaitForSeconds(1.1f);
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