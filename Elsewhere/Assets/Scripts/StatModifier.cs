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
    public int duration;

    public StatModifier(float value, StatModType type, int order, object source, int duration) {
        this.value = value;
        this.type = type;
        this.order = order;
        this.source = source;
        this.duration = duration;
    }
    
    // constructor that requires user to only input value and type
    // this () will auto call the other constructer
    // (int) type is the default value for the order
    // every enum value is auto assigned to an int value (index)
    // duration is the number of turns status effects last
    public StatModifier(float value, StatModType type) : this (value, type, (int) type, null, 0) { }

    public StatModifier(float value, StatModType type, int order) : this (value, type, order, null, 0) { }

    public StatModifier(float value, StatModType type, object source) : this (value, type, (int) type, source, 0) { }

    public StatModifier(float value, int duration, StatModType type) : this(value, type, (int)type, null, duration) { }
}
