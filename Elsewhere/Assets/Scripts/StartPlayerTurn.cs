/** currUnit.startTurn() -> update current tile + updates booleans
 * Find selectable tiles 
 */


using UnityEngine;
using System.Collections;

public class StartPlayerTurn : State
{
    public StartPlayerTurn(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        currUnit.StartTurn();
        turnScheduler.playerActionPanel.SetActive(true);
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].baseValue);

        // menu will be available for viewing...

        yield break;
    }

    public override IEnumerator Attack()
    {
        if (currUnit.currState == UnitState.IDLING) 
        {
            turnScheduler.SetState(new PlayerAttack(turnScheduler));
        }
        yield break;
    }

    public override IEnumerator Ability()
    {
        if (currUnit.currState == UnitState.IDLING)
        {
            turnScheduler.SetState(new PlayerAbility(turnScheduler));
        }
        yield break;
    }

}
