/** Description of AI:
 * If able to heal, retreat from enemies and use item
 * Internal state turns to "help" mode →
 * Retreat towards an allied healer(only if unit can stay out of enemy range)
 * Retreat to nearest fort(only if unit can stay out of enemy range)
*/

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class RecoveryEnemyAI : EnemyState
{
    public RecoveryEnemyAI(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {

        // if can self heal
        if (enemyUnit.CanSelfHeal())
        {
            // retreat, heal
            // from all player positions, find selectable tiles using dijkstra - add a proximity score to each one of them. 
            // Then after that, enemy unit finds selectable tiles and chooses a tile at random of the least proximity score..
            yield return new WaitForSecondsRealtime(0.5f);

            Retreat();

            yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);

            map.RemoveSelectableTiles(currUnit.currentTile);

            // heal

            // randomly assign ability (in future maybe prioritise highest heal instead)
            Ability healAbility = enemyUnit.selfHealingAbilities[new System.Random().Next(0, enemyUnit.selfHealingAbilities.Count)];

            enemyUnit.chosenAbility = healAbility;
            currUnit.CurrState = UnitState.TARGETING;
            map.FindAttackableTiles(currUnit.currentTile, healAbility.attackRange, healAbility.targetingStyle);
            yield return new WaitForSecondsRealtime(1);
            map.RemoveAttackableTiles();

            // TODO in future need to migrate this to abiltyTargeting to maximise the number of targets
            List<Unit> targetUnits = new List<Unit>() { currUnit };

            yield return turnScheduler.StartCoroutine(turnScheduler.AbilityAnimation(currUnit));
            yield return turnScheduler.StartCoroutine(turnScheduler.currUnit.chosenAbility.Execute(turnScheduler.currUnit, targetUnits));

            foreach (Unit unit in targetUnits)
            {
                if (unit.isDead())
                {
                    yield return turnScheduler.StartCoroutine(turnScheduler.RemoveUnit(unit));
                }
                else
                {
                    unit.UpdateUI();
                }
            }

            targetUnits.Clear();

            turnScheduler.SetState(new EnemyEndTurn(turnScheduler));

            yield break;
        }

        else
        {
            List<EnemyUnit> healers = enemies.FindAll(x => x.CanTeamHeal());
            // find closest unit who can team heal
            if (healers.Count > 0)
            {
                List<PlayerUnit> players = turnScheduler.players;

                // Rank healers and find one to run towards
                // Ranked according to distance
                // Ranked by maximising the minimum (distance from player - player range)


                // initialize min/max ally/enemies distance, 
                AStarSearch.GeneratePath(map, enemyUnit.currentTile, healers[0].currentTile, false, true);
                List<int> allyDistances = new List<int>();
                List<float> minPlayerDistances = new List<float>();


                // loop through all the healers and players and rank heuristic 
                foreach (EnemyUnit healer in healers)
                {
                    AStarSearch.GeneratePath(map, enemyUnit.currentTile, healer.currentTile, false, true);
                    allyDistances.Add(healer.currentTile.distance);

                    // finding (distance from player - player range) for all players
                    // moveTo is to tile the player will end up at
                    Tile moveTo = healer.currentTile;
                    bool changed = false;
                    while (moveTo.distance > enemyUnit.stats[StatString.MOVEMENT_RANGE].Value)
                    {
                        moveTo = moveTo.parent;
                        changed = true;
                    }
                    if (changed) Assert.AreNotEqual(moveTo, healer.currentTile);

                    // calculating the minimum player distance and add the inverse since we want to be far away TODO DIKJSTRA ONCE AND FOR ALL 
                    int minPlayerDistance = int.MaxValue;
                    foreach (Unit player in players)
                    {
                        AStarSearch.GeneratePath(map, player.currentTile, moveTo, false, true);
                        minPlayerDistance = Math.Min(minPlayerDistance, (int)(moveTo.distance - player.stats[StatString.MOVEMENT_RANGE].Value));
                    }
                    minPlayerDistances.Add(1.0f / minPlayerDistance);
                }


                // normalize both lists then find the min heustistic (60% ally 40% player)
                List<float> allyDistancesNorm = NormalizeList(allyDistances);
                List<float> minPlayerDistancesNorm = NormalizeList(minPlayerDistances);

                float minRecoveryHeuristic = float.MaxValue;
                int targetHealerIndex = 0;

                for (int i = 0; i < allyDistancesNorm.Count; i++)
                {
                    float currentHeuristic = 0.6f * allyDistancesNorm[i] + 0.4f * minPlayerDistancesNorm[i];
                    if (currentHeuristic < minRecoveryHeuristic)
                    {
                        targetHealerIndex = i;
                        minRecoveryHeuristic = currentHeuristic;
                    }
                }


                // go to closest healer
                Tile targetTile = healers[targetHealerIndex].currentTile;

                AStarSearch.GeneratePath(map, enemyUnit.currentTile, targetTile, false, true);

                map.FindSelectableTiles(enemyUnit.currentTile, enemyUnit.stats[StatString.MOVEMENT_RANGE].Value);

                yield return new WaitForSecondsRealtime(0.5f);

                /*
                while (targetTile.distance > enemyUnit.stats[StatString.MOVEMENT_RANGE].Value)
                {
                    targetTile = targetTile.parent;
                }
                */
                while (!targetTile.selectable)
                {
                    targetTile = targetTile.parent;
                }
                enemyUnit.GetPathToTile(targetTile);

                yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);

                turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
            }

            // no healing avenues. Run? Or change to aggressive mode
            else
            {
                yield return new WaitForSecondsRealtime(0.5f);

                Retreat();

                yield return new WaitUntil(() => currUnit.CurrState == UnitState.IDLING);

                map.RemoveSelectableTiles(currUnit.currentTile);

                turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
            }
        }

        yield break;
    }

    public override IEnumerator Attack()
    {
        Unit targetPlayer = currUnit.attackingTargetUnit;

        map.RemoveSelectableTiles(currUnit.currentTile, false);
        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].Value) ;
        // should display the attacking tiles.

        yield return new WaitForSecondsRealtime(1);

        map.RemoveAttackableTiles();
        currUnit.StartAttack(targetPlayer);
        BattleManager.Battle(currUnit, targetPlayer);

        // TODO how can like this?
        yield return turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, targetPlayer));

        turnScheduler.SetState(new EnemyEndTurn(turnScheduler));
    }


    private static List<float> NormalizeList(List<float> list)
    {
        float max = list.Max();
        float min = list.Min();
        return list.ConvertAll(x => (x - min) / (max - min));
    }
    private static List<float> NormalizeList(List<int> list)
    {
        float max = list.Max();
        float min = list.Min();
        return list.ConvertAll(x => (x - min) / (max - min));
    }

    private void Retreat()
    {
        int[,] proximityList = new int[map.mapSize.x, map.mapSize.y];
        foreach (Unit player in turnScheduler.players)
        {
            map.FindSelectableTiles(player.currentTile, player.stats[StatString.MOVEMENT_RANGE].Value);
            foreach (Tile tile in map.GetSelectableTiles())
            {
                proximityList[tile.gridPosition.x, tile.gridPosition.y]++;
            }
            map.RemoveSelectableTiles(player.currentTile);
        }

        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats[StatString.MOVEMENT_RANGE].Value);


        List<Tile> targetTiles = new List<Tile>();
        int minProximity = proximityList[0, 0];
        foreach (Tile tile in map.GetSelectableTiles())
        {
            int newEstimate = proximityList[tile.gridPosition.x, tile.gridPosition.y];
            if (newEstimate < minProximity)
            {
                minProximity = newEstimate;
                targetTiles.Clear();
                targetTiles.Add(tile);
            }
            else if (newEstimate == minProximity)
            {
                targetTiles.Add(tile);
            }
        }
        int tileSelectedIdx = new System.Random().Next(0, targetTiles.Count);
        Tile targetTile = targetTiles[tileSelectedIdx];

        enemyUnit.GetPathToTile(targetTile);
    }
}

