using JetBrains.Annotations;
using System.Collections;

public class WaitEnemyAI : EnemyState
{
    public WaitEnemyAI(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        // find out if there are units in attackable range
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);

        // find the attackable tiles in total
        foreach (Tile tile in map.GetSelectableTiles())
        {
            // check that it's not an internal
            bool insideTile = true;

            int[] hor = { -1, 0, 1, 0 };
            int[] vert = { 0, 1, 0, -1 };

            int currX = tile.gridPosition.x, currY = tile.gridPosition.y;

            for (int j = 0; j < 4; j++)
            {
                int newX = currX + hor[j];
                int newY = currY + vert[j];
                if (newX >= 0 && newX < map.mapSize.x && newY >= 0 && newY < map.mapSize.y)
                {
                    if (!map.tileList[newX][newY].selectable)
                    {
                        insideTile = false;
                    }
                }
            }

            // if not internal check attackableTiles
            if (!insideTile)
            {
                map.FindAttackableTiles(tile, currUnit.stats[StatString.ATTACK_RANGE].Value, TargetingStyle.SINGLE);
            }
        }

        bool targetInRange = false;
        foreach(Tile tile in map.GetAttackableTiles())
        {
            if (turnScheduler.players.Find(x => x.currentTile == tile) != null)
            {
                targetInRange = true;
                break;
            }
        }
        map.RemoveAttackableTiles();
        map.RemoveSelectableTiles(currUnit.currentTile);

        if (targetInRange)
        {
            turnScheduler.SetState(new EnemyAiAggressive(turnScheduler));
        }
        else
        {
            turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
        }


        yield break;
    }

}