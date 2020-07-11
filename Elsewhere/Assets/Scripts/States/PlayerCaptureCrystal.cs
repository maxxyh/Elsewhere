using System.Collections;
using UnityEngine;

namespace States
{
    public class PlayerCaptureCrystal : State
    {
        public PlayerCaptureCrystal(TurnScheduler turnScheduler) : base(turnScheduler)
        {
            
        }

        public override IEnumerator Execute()
        {
            turnScheduler.playerActionPanel.SetActive(false);
            map.RemoveSelectableTiles(currUnit.currentTile);
            Unit.OnCaptureCrystal(currUnit);
            // need to check if there was convo
            
            // TODO fix crystal boost
            
            // need to wait until dialogue is over before ending turn + weird yellow bug
            // little bubble appears on top of players 
            
            turnScheduler.StartCoroutine(turnScheduler.AbilityAnimation(turnScheduler.currUnit));
            yield break;
        }

        public override IEnumerator CrystalCaptureCutSceneDone()
        {
            yield return new WaitUntil(() => !currUnit.anim.GetBool("isAbility"));
            yield return new WaitForSeconds(1f);
            turnScheduler.SetState(new PlayerEndTurn(turnScheduler));
        }
    }
}