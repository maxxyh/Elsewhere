using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class EnemyAiCautiousMovement : EnemyState
{
    public EnemyAiCautiousMovement(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);

        yield return new WaitForSecondsRealtime(0.25f);

        List<Tile> targets = turnScheduler.players.ConvertAll(x => x.currentTile);
        Tile targetTile = AStarSearch.GeneratePathToNearestTarget(map, currUnit.currentTile, targets, false, true);
        Unit targetPlayer = turnScheduler.players.Find(x => x.currentTile == targetTile);

        // check if target tile is selectable, and also go as far from movement range as possible 
        int cautiousRange = 2;
        while (!targetTile.selectable || targetTile.distance > cautiousRange)
        {
            targetTile = targetTile.parent;
        }
        
        // A star movement towards the target, 70% of the time, 10% of time will choose a random target and go
        int chance = new Random().Next(1, 100);
        if (chance <= 70)
        {
            currUnit.GetPathToTile(targetTile);
        } 
        else if (chance >= 90)
        {
            List<Tile> closeTiles = map.GetSelectableTiles().ToList().FindAll(tile => tile.distance <= 2);
            targetTile = closeTiles[new Random().Next(0, closeTiles.Count - 1)];
            currUnit.GetPathToTile(targetTile);
        }

        yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);
        
        turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
    }
}
