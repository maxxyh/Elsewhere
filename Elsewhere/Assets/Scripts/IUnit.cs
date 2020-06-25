/** A unit can move and attack 
 * 
 */

using System.Collections;
using System.Collections.Generic;

public interface IUnit
{
    UnitState CurrState { get; set; }
    Dictionary<StatString, UnitStat> stats { get; set; }
    string characterClass { get; set; }

    IEnumerator AbilityAnimation();
    void AssignAbilities(List<Ability> abilities);
    void AssignMap(Map map);
    void AssignStats(Dictionary<StatString, float> input);
    void AssignStats(Dictionary<StatString, string> input);
    IEnumerator AttackAnimation();
    void DecrementAllStatDuration();
    void EndTurn();
    void GetPathToTile(Tile target);
    bool isDead();
    void MakeInactive();
    void Move();
    void MovementAnimation();
    void OnMouseDown();
    void RemoveGrayscale();
    void ReturnToStartTile();
    void Start();
    void StartAttack(Unit unit);
    void StartTurn();
    void TakeDamage(float damage);
    void UpdateUI();
}