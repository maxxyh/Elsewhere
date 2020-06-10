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
        map.FindSelectableTiles(currUnit.currentTile, currUnit.stats["movementRange"].Value);

        // menu will be available for viewing...

        yield break;
    }

    public override IEnumerator Targeting(ActionType actType)
    {
        if (currUnit.currState == UnitState.IDLING) 
        {
            if (actType == ActionType.ATTACK)
            {
                turnScheduler.SetState(new PlayerAttackTargeting(turnScheduler));
            }
            else if (actType == ActionType.ABILITY)
            {
                turnScheduler.SetState(new PlayerAbilityTargeting(turnScheduler));
            }
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
