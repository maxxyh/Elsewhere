using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public BattleState state;
    void Start()
    {
        state = BattleState.START;
        // SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
