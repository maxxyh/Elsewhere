using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // this Dictionary string = team name, contains all units in the team.
    public static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    // turnKey stores team order
    static Queue<string> turnKey = new Queue<string>();
    // Queue for current team
    static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    // add the next team into the turn queue
    static void InitTeamTurnQueue()
    {
        List<TacticsMove> teamList = units[turnKey.Peek()];
        foreach(TacticsMove unit in teamList)
        {
            turnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    public static void StartTurn()
    {
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }


    // removes unit from team queue then continues with next player OR goes to next team
    public static void EndTurn()
    {
        TacticsMove unit = turnTeam.Dequeue();
        unit.EndTurn();

        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;

        if (!turnKey.Contains(unit.tag))
        {
            turnKey.Enqueue(unit.tag);
        }

        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
            units[unit.tag] = list;
        }
        else
        {
            list = units[unit.tag];
        }

        list.Add(unit);

        // TODO implement a RemoveUnit and if it's the last member it will also Remove the team - die.
    }
}
