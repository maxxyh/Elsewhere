using UnityEngine;
using System.Collections;

public class TutPlayerAttackTargeting : PlayerAttackTargeting
{
    public TutPlayerAttackTargeting(TurnScheduler turnScheduler) : base(turnScheduler) { }

    public override IEnumerator Execute()
    {
        turnScheduler.moveDialogue.SetActive(false);
        turnScheduler.attackDialogue.SetActive(true);
        turnScheduler.playerActionPanel.SetActive(false);
        turnScheduler.cancelPanel.SetActive(true);

        map.FindAttackableTiles(currUnit.currentTile, currUnit.stats[StatString.ATTACK_RANGE].Value);
        // should display the attacking tiles.
        currUnit.CurrState = UnitState.TARGETING;

        yield break;

    }
    public override IEnumerator Attack()
    {
        turnScheduler.SetState(new TutPlayerAttack(turnScheduler));
        yield break;
    }

    public override IEnumerator Cancel()
    {
        turnScheduler.playerActionPanel.SetActive(true);
        turnScheduler.cancelPanel.SetActive(false);
        map.RemoveAttackableTiles();

        turnScheduler.SetState(new TutPlayerUnitSelected(turnScheduler));
        yield break;
    }

}
