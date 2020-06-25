using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyState : State
{
    public EnemyUnit enemyUnit;
    public List<EnemyUnit> enemies;
    public EnemyState(TurnScheduler turnScheduler) : base(turnScheduler)
    {
        enemyUnit = turnScheduler.enemies.Find(x => x.currentTile == currUnit.currentTile);
        enemies = turnScheduler.enemies;
    }
}
