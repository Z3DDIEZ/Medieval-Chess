namespace MedievalChess.Domain.Primitives;

public enum LoyaltyState
{
    Defecting, // 0-29
    Disloyal,  // 30-49
    Wavering,  // 50-69
    Loyal,     // 70-89
    Devoted    // 90-100
}

public readonly struct LoyaltyValue : IEquatable<LoyaltyValue>
{
    public int Value { get; }

    public LoyaltyValue(int value)
    {
        Value = Math.Clamp(value, 0, 100);
    }

    public LoyaltyState State => Value switch
    {
        >= 90 => LoyaltyState.Devoted,
        >= 70 => LoyaltyState.Loyal,
        >= 50 => LoyaltyState.Wavering,
        >= 30 => LoyaltyState.Disloyal,
        _ => LoyaltyState.Defecting
    };

    public LoyaltyValue Adjust(int delta) => new(Value + delta);

    public bool IsDefecting => State == LoyaltyState.Defecting;
    public bool IsWaveringOrWorse => Value < 70;

    public override bool Equals(object? obj) => obj is LoyaltyValue other && Equals(other);
    public bool Equals(LoyaltyValue other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(LoyaltyValue left, LoyaltyValue right) => left.Equals(right);
    public static bool operator !=(LoyaltyValue left, LoyaltyValue right) => !left.Equals(right);
    public static implicit operator int(LoyaltyValue l) => l.Value;
    public static implicit operator LoyaltyValue(int v) => new(v);
}
