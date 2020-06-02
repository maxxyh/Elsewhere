/** A unit can move and attack 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

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
    public float currHP;

    [Header("States")]
    public bool takingTurn = false;
    public bool moving = false;
    public float moveSpeed;
    public bool attackingPhase = false;
    public bool isAttacking = false;

    [Header("References")]
    public GameObject Panel;
    public Unit attackingTargetUnit;

    // Movement Variables
    public Map map;
    
    Stack<Tile> path = new Stack<Tile>();
    public Tile currentTile;

    Vector3 heading = new Vector3();
    Vector3 velocity = new Vector3();
    private Vector2 lastMove;

    public Rigidbody2D rb;

    // Dictionary style constructor 
    public void AssignStats(Dictionary<string,float> input)
    {
        stats = new Dictionary<string, UnitStat>();
        foreach(KeyValuePair<string,float> pair in input)
        {
            this.stats[pair.Key] = new UnitStat(pair.Value);
        }
        currHP = this.stats["HP"].CalculateFinalValue();
    }

    public void AssignMap(Map map)
    {
        this.map = map;
    }

    public void Start()
    {
        //map = FindObjectOfType<Map>();
    }

    // same as Update but update frame in a fixed time 50 times/s
    public void Update()
    {

    }

    void FixedUpdate()
    {
         
    }
    // draft
    public bool isDead()
    {
        return currHP <= 0;
    }

    // sets the currentTile 
    public void StartTurn()
    {
        currentTile = map.GetCurrentTile(transform.position);
        currentTile.isStartPoint = true;
        takingTurn = true;
    }

    public void EndTurn()
    {
        //map.RemoveSelectedTiles(currentTile); HANDLED IN ENDACTION...
        takingTurn = false;
    }
    

    public void StartAttack(Unit unit)
    {
        moving = false;
        isAttacking = true;
        attackingTargetUnit = unit;
    }

    public void TakeDamage(float damage) {
        stats["HP"].baseValue -= damage;
        currHP = stats["HP"].CalculateFinalValue();
        
        // print ("damage done: " +  damage);
        // print ("my HP: " + currHP);
        // if (this.HP.baseValue <= 0) {
        //     isDead = true;
        //     this.gameObject.tag = "DeadPlayerUnit";
        //     Destroy(this.gameObject);
        // }
    }

    public void BasicAttack()
    {
        float distance = Vector3.Distance(transform.position, attackingTargetUnit.transform.position);
        if (distance <= this.stats["attackRange"].baseValue)
        {
            attackingTargetUnit.TakeDamage(this.stats["attackDamage"].baseValue);
        }
        else
        {
            print("Enemy not in range");
        }
        // https://www.youtube.com/watch?v=Hp765p29YtE
    }

    // Generates the path to the tile
    public void GetPathToTile(Tile target)
    {
        AStarSearch.GeneratePath(map, currentTile, target, true);
        path.Clear();
        target.target = true;
        moving = true;

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
            if (Vector3.Distance(transform.position, target) >= 0.02f)
            {
                //print("heading: " + heading);
                //print("target: " + target);
                CalculateHeading(target);
                MovementAnimation();
                SetHorizontalVelocity();

                //transform.up = heading;
                // TODO use or REMOVE heading. doesn't seem to be required yet.
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
            currentTile.isStartPoint = false;
            currentTile.occupied = false;
            currentTile.selectable = true;
            currentTile.target = false;
            
            moving = false;

            // update currentTile
            currentTile = map.GetCurrentTile(transform.position);
            currentTile.isStartPoint = true;
            
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
        if (panel != null)
        {
            if (!panel.activeSelf)
            {
                panel.SetActive(true);
            } 
            else
            {
                panel.SetActive(false);
            }
        }
    }

    public void MovementAnimation()
    {
        /*
        float horizontalMove = heading.x * moveSpeed;
        float verticalMove = moveSpeed;
        if ((heading.x == -1 || heading.x == 1) && heading.y == 0)
        {
            anim.SetFloat("moveSpeed", Mathf.Abs(horizontalMove));
        } 
        else if ((heading.y == -1 || heading.y == 1) && heading.x == 0)
        {
            anim.SetFloat("moveSpeed", verticalMove);
        }
        */
        
        //rb.velocity = heading * moveSpeed;
        if (moving)
        {
            /*
            anim.SetFloat("moveSpeed", moveSpeed);
            
            // move up
            if (heading.y == 1)
            {
                anim.SetBool("isFacingLeft", false);
                anim.SetBool("isFacingUp", true);
            }
            
            // move down
            else if (heading.y == -1)
            {
                anim.SetBool("isFacingLeft", false);
                anim.SetBool("isFacingUp", false);
            }
            else if (heading.x == 1 || heading.x == -1)
            {
                anim.SetBool("isFacingLeft", true);
                
            } 
            */
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
}
