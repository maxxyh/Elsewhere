
using System.Collections;
using System.Collections.Generic;

public class AbilityWallShatter : Ability
{
    public AbilityWallShatter() : base("Wall Shatter", 1, 5, false, TargetingStyle.OBSTACLES, 
        new[] {AbilityType.BUFF})
    {
    }
    
    public IEnumerator Execute(Unit initiator, Tile target, Map map)
    {
        {
            map.RemoveAllObstaclesFromTile(target);
            DamagePopUp.Create(target.transform.position, "Wall Break", PopupType.DEBUFF);
        }
        initiator.UpdateUI();

        yield break;
    }
    
}