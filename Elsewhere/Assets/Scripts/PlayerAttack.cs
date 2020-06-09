using UnityEngine;
using System.Collections;

/*
public class PlayerAttack : State
{
    public PlayerAttack(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        Unit targetUnit = null;
        
        // wait for player to click on a valid target
        yield return new WaitUntil(() =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("tile"))
                    {
                        Tile t = hit.collider.GetComponent<Tile>();
                        foreach (Unit unit in turnScheduler.enemies)
                        {
                            if (unit.currentTile == t)
                            {
                                targetUnit = unit;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        });
        
        map.RemoveAttackableTiles();

        // taking a risk here...targetUnit might be null apparently! Trust the WaitUntil.
        currUnit.StartAttack(targetUnit);
        BattleManager.Battle(currUnit, targetUnit);
        
        // TODO how can like this?
        yield return turnScheduler.StartCoroutine(turnScheduler.AttackAnimation(currUnit, targetUnit));

        turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
    }

    
}
*/