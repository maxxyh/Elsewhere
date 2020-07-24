using System.Collections.Generic;
using LevelSelection;

public static class StaticData
{
    private static Dictionary<string, Ability> _abilityReference = new Dictionary<string, Ability>()
    {
        {"HealingWave", new AbilityHealingWave()},
        {"ArcaneBoost", new AbilityArcaneBoost()},
        {"HpReaver", new AbilityHPReaver()},
        {"AstralFlare", new AbilityAstralFlare()},
        {"DoubleHit", new AbilityDoubleHit()},
        {"WhirlwindSlash", new AbilityWhirlwindSlash()},
        {"IonicStrike", new AbilityIonicStrike()},
        {"WallShatter", new AbilityWallShatter()},
        {"DeadlyRicochet", new AbilityDeadlyRicochet()},
        {"FinalHour", new AbilityFinalHour()}
    };

    public static Dictionary<string, Ability> AbilityReference
    {
        get => _abilityReference;
        set => _abilityReference = value;
    }

    public static List<string> SelectedUnits = new List<string>();

    public static LevelDatabaseEntry LevelInformation;
}
