 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : Unit
{

    #region AI fields

    public List<Ability> selfHealingAbilities;
    public List<Ability> teamHealingAbilities;
    public bool inRecoveryMode;
    public float recoveryEntrancePercentage;
    public float recoveryExitPercentage;
    public bool hasWaitingMode = true;

    #endregion

    public bool HasWaitingMode()
    {
        return this.hasWaitingMode;
    }

    public void InitAIAbilityInfo()
    {
        selfHealingAbilities = abilities.FindAll(x => x.abilityTypes.Contains(AbilityType.HEAL_SELF));
        teamHealingAbilities = abilities.FindAll(x => x.abilityTypes.Contains(AbilityType.HEAL_TEAM));
        inRecoveryMode = false;
    }

    public void AssignRecoveryMode(float enter = 0.3f, float exit = 0.8f)
    {
        recoveryEntrancePercentage = enter;
        recoveryExitPercentage = exit;
    }
    public override void AssignAbilities(List<Ability> abilities)
    {
        this.abilities = abilities;
        InitAIAbilityInfo();
    }

    public bool CanSelfHeal()
    {
        // in future if there are items, can add that to the check as well.
        return this.selfHealingAbilities.Count > 0;
    }

    public bool CanTeamHeal()
    {
        return this.teamHealingAbilities.Count > 0;
    }

    public void CheckIfRecoveryMode()
    {
        // was already in recovery mode
        if (inRecoveryMode)
        {
            // check that is above recoveryExitPercentage
            if (this.stats[StatString.HP].Value > recoveryExitPercentage * this.stats[StatString.HP].baseValue)
            {
                inRecoveryMode = false;
            }
        }
        // not originally in recovery mode
        else
        {
            // check that is below recoveryEntrancePercentage
            if (this.stats[StatString.HP].Value < recoveryEntrancePercentage * this.stats[StatString.HP].baseValue)
            {
                this.inRecoveryMode = true;
            }
        }
    }





    // Update is called once per frame
    private void Update()
    {
        // simply update currentTile if not taking turn
        if (CurrState == UnitState.ENDTURN)
        {
            if (currentTile == null && map != null)
            {
                currentTile = map.GetCurrentTile(transform.position);
            }
            return;
        }

        if (CurrState == UnitState.MOVING)
        {
            Move();
        }
    }
 }
