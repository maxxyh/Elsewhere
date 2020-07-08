using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using UnityEditorInternal;

public class Level
{
    public int requiredExperience;
    public int currentExperience;
    public int currentLevel;
    public Action OnLevelUp;

    private readonly int BASE_EXP = 100;
    private readonly float EXPONENT = 1.1f;
    public readonly int MAX_LEVEL = 50;

    public Level(int level, int currentExperience, Action OnLevelUp)
    {
        currentLevel = level;
        requiredExperience = GetLevelExpRequired(level);
        this.currentExperience = currentExperience;
        this.OnLevelUp = OnLevelUp;
    }

    private int GetLevelExpRequired(int level)
    {
        if (level >= MAX_LEVEL)
        {
            return 0;
        }
        else
        {
            return (int)Math.Floor(BASE_EXP * Math.Pow(EXPONENT,(level - 1)));
        }
    }

    public bool AddExp(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Tried to add zero or less than zero exp");
        }
        
        bool levelUp = false;
        currentExperience += amount;
        while (currentExperience >= requiredExperience)
        {
            levelUp = true;
            currentLevel++;
            currentExperience -= requiredExperience;
            requiredExperience = GetLevelExpRequired(currentLevel);
            if (OnLevelUp != null)
            {
                OnLevelUp.Invoke();
            }
        }
        Debug.Log($"{amount} exp added. Current exp = {currentExperience}");
        return levelUp;
    }

    public static int CalculateExp(Level TargetLevel, Level InitiatorLevel, bool killed, bool targetsOwnTeam = false)
    {
        int modeBonus = 8;
        int supportBaseValue = 17;

        int targetLevel = TargetLevel.currentLevel;
        int initiatorLevel = InitiatorLevel.currentLevel;

        // support ability
        if (targetsOwnTeam)
        {
            return supportBaseValue - Math.Max(initiatorLevel - 5, 0) / 3 + modeBonus;
        }
        // damage ability
        else
        {
            int levelDifference = targetLevel - initiatorLevel;
            return CalculateBattleExp(levelDifference, killed);
        }
    }

    private static int CalculateBattleExp(int levelDifference, bool killed)
    {
        if (killed)
        {
            if (levelDifference >= 0)
            {
                return 20 + (levelDifference * 3);
            } 
            else if (levelDifference == -1)
            {
                return 20;
            }
            else
            {
                return Math.Max(26 + (levelDifference * 3), 7);
            }
        }

        else
        {
            if (levelDifference >= 0)
            {
                return (31 + levelDifference) / 3;
            }
            else if (levelDifference == -1)
            {
                return 10;
            }
            else
            {
                return Math.Max((33 + levelDifference) / 3, 1);
            }
        }
    }
}
