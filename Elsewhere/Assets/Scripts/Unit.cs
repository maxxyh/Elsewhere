/** A unit can move and attack 
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{

    #region FIELDS AND REFERENCES
    [Header("UI")]
    public GameObject statPanel;
    public Text unitName;
    public Text unitHP;
    public Text unitMana;
    public Text unitAttackDamage;
    public Text unitMagicDamage;
    public Text unitArmor;
    public Text unitMagicRes;
    public Text unitMovementRange;
    public Text unitAttackRange;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();
    public Ability chosenAbility;

    [Header("Identifiers")]
    public int unitID;
    public string characterName;

    public Animator anim;
    /* All stats: 
     * attackDamage;
     * magicDamage;
     * mana;
     * HP;
     * defence;
     * movementRange;
     * attackRange;
    */
    [Header("Player Stats")]
    public Dictionary<string, UnitStat> stats;
    public float moveSpeed;

    [Header("States")]
    private UnitState _currState = UnitState.ENDTURN;
    public UnitState currState
    {
        get { return _currState; }

        set { _currState = value;  }
    }

    
    [Header("References")]
    public GameObject Panel;
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

    [Header("For collision")]
    public Transform sparkle;

    #endregion


    // Dictionary style constructor 
    public void AssignStats(Dictionary<string,float> input)
    {
        stats = new Dictionary<string, UnitStat>();
        foreach(KeyValuePair<string,float> pair in input)
        {
            bool hasLimit = false;
            if (pair.Key.Equals("HP") || pair.Key.Equals("mana"))
            {
                hasLimit = true;
            }
            this.stats[pair.Key] = new UnitStat(pair.Value, hasLimit);
            
        }
    }

    public void AssignAbilities(List<Ability> abilities)
    {
        this.abilities = abilities;
    }


    public void AssignMap(Map map)
    {
        this.map = map;
    }

    public void Start()
    {
        ParticleSystem.EmissionModule tempSparkle = sparkle.GetComponent<ParticleSystem>().emission;
        tempSparkle.enabled = false;

        if (currState == UnitState.ENDTURN)
        {
            statPanel.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        unitName.text = this.characterName;
        unitHP.text = this.stats["HP"].Value.ToString() + "/" + this.stats["HP"].baseValue.ToString();
        unitMana.text = this.stats["mana"].Value.ToString() + "/" + this.stats["mana"].baseValue.ToString(); ;
        unitAttackDamage.text = this.stats["attackDamage"].CalculateFinalValue().ToString();
        unitMagicDamage.text = this.stats["magicDamage"].CalculateFinalValue().ToString();
        unitArmor.text = this.stats["armor"].CalculateFinalValue().ToString();
        unitMagicRes.text = this.stats["magicRes"].CalculateFinalValue().ToString();
        unitMovementRange.text = this.stats["movementRange"].CalculateFinalValue().ToString();
        unitAttackRange.text = this.stats["attackRange"].CalculateFinalValue().ToString();
    }

    public bool isDead()
    {
        return this.stats["HP"].Value <= 0;
    }

    // sets the currentTile 
    public void StartTurn()
    {
        startTile = map.GetCurrentTile(transform.position); ;
        currentTile = startTile;
        currState = UnitState.IDLING;
        this.statPanel.SetActive(true);
    }

    public void EndTurn()
    {
        currState = UnitState.ENDTURN;
        this.statPanel.SetActive(false);
    }
    

    public void StartAttack(Unit unit)
    {
        currState = UnitState.ATTACKING;
        attackingTargetUnit = unit;
    }

    public IEnumerator AttackAnimation()
    {
        anim.SetBool("isAttacking", true);
        yield return new WaitForSecondsRealtime(0.4f);
        anim.SetBool("isAttacking", false);
    }

    public IEnumerator AbilityAnimation()
    {
        anim.SetBool("isAbility", true);
        yield return new WaitForSecondsRealtime(0.5f);
        anim.SetBool("isAbility", false);
    }

    public void TakeDamage(float damage) {
        stats["HP"].AddModifier(new StatModifier(-damage, StatModType.Flat));
    }

    #region MOVEMENT

    // Generates the path to the tile
    public void GetPathToTile(Tile target)
    {
        AStarSearch.GeneratePath(map, currentTile, target, true);
        path.Clear();
        target.target = true;
        currState = UnitState.MOVING;

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
            }
        }
        else
        {
            // clear currentTile and make sure it's selectable
            currentTile.Reset();
            currentTile.occupied = false;
            currentTile.selectable = true;

            currState = UnitState.IDLING;

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
        if (statPanel != null)
        {
            if (!statPanel.activeSelf)
            {
                statPanel.SetActive(true);
            } 
            else
            {
                statPanel.SetActive(false);
            }
        }
    }

    public void MovementAnimation()
    {
        if (currState == UnitState.MOVING)
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

    private void OnMouseEnter()
    {
        this.statPanel.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (currState == UnitState.ENDTURN)
        {
            this.statPanel.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("crystal"))
        {
            ParticleSystem.EmissionModule tempSparkle = sparkle.GetComponent<ParticleSystem>().emission;
            tempSparkle.enabled = true;
            StartCoroutine(stopSparkle());
            Destroy(collision.gameObject);
            if (this.gameObject.CompareTag("enemy"))
            {
                this.stats["attackDamage"].AddModifier(new StatModifier(3, StatModType.Flat));
                UpdateUI();
            }
        }
    }

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

    IEnumerator stopSparkle()
    {
        yield return new WaitForSeconds(0.4f);
        ParticleSystem.EmissionModule tempSparkle = sparkle.GetComponent<ParticleSystem>().emission;
        tempSparkle.enabled = false;
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