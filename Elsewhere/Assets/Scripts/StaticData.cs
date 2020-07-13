using System.Collections.Generic;

public static class StaticData
{
    private static Dictionary<string, Ability> _abilityReference = new Dictionary<string, Ability>()
    {
        {"HealingWave", new AbilityHealingWave()},
        {"ArcaneBoost", new AbilityArcaneBoost()},
        {"HpReaver", new AbilityHPReaver()},
        {"AstralFlare", new AbilityAstralFlare()},
        {"DoubleHit", new AbilityDoubleHit()},
        {"WhirlwindSlash", new AbilityWhirlwindSlash()}
    };

    public static Dictionary<string, Ability> AbilityReference
    {
        get => _abilityReference;
        set => _abilityReference = value;
    }

    public static List<string> SelectedUnits = new List<string>();
}
