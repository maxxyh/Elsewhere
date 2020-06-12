public enum StatModType 
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}

public class StatModifier
{
    public readonly float value;
    public readonly StatModType type;
    public readonly int order;
    public readonly object source;

    public StatModifier(float value, StatModType type, int order, object source) {
        this.value = value;
        this.type = type;
        this.order = order;
        this.source = source;
    }
    
    // constructor that requires user to only input value and type
    // this () will auto call the other constructer
    // (int) type is the default value for the order
    // every enum value is auto assigned to an int value (index)
    public StatModifier(float value, StatModType type) : this (value, type, (int) type, null) { }

    public StatModifier(float value, StatModType type, int order) : this (value, type, order, null) { }

    public StatModifier(float value, StatModType type, object source) : this (value, type, (int) type, source) { }
}
