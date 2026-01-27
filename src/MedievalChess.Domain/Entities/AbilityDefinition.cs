using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Entities;

/// <summary>
/// Static definition of an ability. Immutable template.
/// </summary>
public class AbilityDefinition
{
    public AbilityType Type { get; }
    public string Name { get; }
    public string Description { get; }
    public AbilityTier Tier { get; }
    public int APCost { get; }
    public int Cooldown { get; }
    public int XPRequired { get; }
    public PieceType RequiredPieceType { get; }
    public bool RequiresTarget { get; }
    public int Range { get; }

    public AbilityDefinition(
        AbilityType type,
        string name,
        string description,
        AbilityTier tier,
        int apCost,
        int cooldown,
        int xpRequired,
        PieceType requiredPieceType,
        bool requiresTarget = false,
        int range = 1)
    {
        Type = type;
        Name = name;
        Description = description;
        Tier = tier;
        APCost = apCost;
        Cooldown = cooldown;
        XPRequired = xpRequired;
        RequiredPieceType = requiredPieceType;
        RequiresTarget = requiresTarget;
        Range = range;
    }
}
