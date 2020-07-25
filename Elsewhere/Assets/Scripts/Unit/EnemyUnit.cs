 using System.Collections;
using System.Collections.Generic;
using System.Linq;
 using Newtonsoft.Json.Linq;
 using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : Unit
{

    #region AI fields

    public List<Ability> selfHealingAbilities;
    public List<Ability> teamHealingAbilities;
    private bool inRecoveryMode;
    public float recoveryEntrancePercentage;
    public float recoveryExitPercentage;
    public bool hasWaitingMode;
    public bool hasRecoveryMode;

    public EnemyUnit medicTarget;
    public int distanceToMedicTarget = int.MaxValue;

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
    
    public override void AssignAbilities(IEnumerable<string> abilityNames, JObject abilityConfig)
    {
        abilities = new List<Ability>();
        foreach (string abilityName in abilityNames)
        {
            abilities.Add(StaticData.AbilityReference[abilityName]);
            majorStatPanel.AddAbilityToPanel((string) abilityConfig[abilityName]["name"], 
                (string) abilityConfig[abilityName]["description"], StaticData.AbilityReference[abilityName].GetManaCost());
        }
        InitAIAbilityInfo();
    }
    
    public override void AssignAbilities(List<Ability> abilities)
    {
        Debug.Log("EnemyAssignAbilities");
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

    public bool IsRecoveryMode()
    {
        Debug.Log($"current HP = {stats[StatString.HP].Value}, full HP = {stats[StatString.HP].baseValue}");
        
        if (!hasRecoveryMode)
        {
            return false;
        }

        if (inRecoveryMode)
        {
            if (stats[StatString.HP].Value >= recoveryExitPercentage * stats[StatString.HP].baseValue)
            {
                inRecoveryMode = false;
            }
        }
        else
        {
            if (stats[StatString.HP].Value <= recoveryEntrancePercentage * stats[StatString.HP].baseValue)
            {
                inRecoveryMode = true;
            }
        }
        Debug.Log($"inRecoveryMode = {inRecoveryMode}");
        return inRecoveryMode;

        /*// was already in recovery mode
        if (inRecoveryMode)
        {
            // check that is above recoveryExitPercentage
            if (stats[StatString.HP].Value > recoveryExitPercentage * stats[StatString.HP].baseValue)
            {
                inRecoveryMode = false;
                return false;
            }
            else
            {
                return true;
            }
        }
        // not originally in recovery mode
        else
        {
            // check that is below recoveryEntrancePercentage
            if (stats[StatString.HP].Value < recoveryEntrancePercentage * stats[StatString.HP].baseValue)
            {
                inRecoveryMode = true;
                return true;
            }
            
            return false;
        }*/


    }

    public bool IsMedic() 
    {
        return hasRecoveryMode && medicTarget != null;
    }
    
    public void ResetMedicStatus()
    {
        medicTarget = null;
        distanceToMedicTarget = int.MaxValue;
    }

    public void AssignWaitAndRecovery(bool wait, bool recovery)
    {
        hasWaitingMode = wait;
        hasRecoveryMode = recovery;
        if (recovery)
            AssignRecoveryMode();
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
