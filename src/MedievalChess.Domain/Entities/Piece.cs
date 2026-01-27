using MedievalChess.Domain.Common;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public abstract class Piece : AggregateRoot<Guid>
{
    public abstract PieceType Type { get; }
    public PlayerColor Color { get; private set; }
    public Position? Position { get; internal set; } // Null if captured
    public LoyaltyValue Loyalty { get; internal set; }
    
    // Progression
    public int Level { get; protected set; }
    public int XP { get; protected set; }
    
    // Combat (Attrition Mode)
    public int MaxHP { get; protected set; }
    public int CurrentHP { get; protected set; }
    
    // New Collections
    public List<PieceAbility> Abilities { get; private set; } = new();
    public List<ActiveEffect> ActiveEffects { get; private set; } = new();
    
    public bool IsCaptured => Position == null;

    protected Piece(PlayerColor color, Position position)
    {
        Id = Guid.NewGuid();
        Color = color;
        Position = position;
        Loyalty = new LoyaltyValue(80); // Default Loyal
        Level = 1;
        XP = 0;
        // HP Initialization depends on concrete class, will be set in constructor or init
    }

    protected Piece() { } // EF Core requirement

    public void MoveTo(Position newPosition)
    {
        if (IsCaptured) throw new InvalidOperationException("Captured piece cannot move");
        Position = newPosition;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Math.Max(0, CurrentHP - damage);
        if (CurrentHP == 0)
        {
            Position = null; // Captured
        }
    }

    /// <summary>
    /// Standard chess capture (no damage - instant removal)
    /// </summary>
    public void Capture()
    {
        Position = null;
    }

    /// <summary>
    /// Track if piece has moved (for castling rights)
    /// </summary>
    public bool HasMoved { get; private set; }

    public void MarkAsMoved()
    {
        HasMoved = true;
    }

    public void GainXP(int amount)
    {
        XP += amount;
        // Simple level up logic for now: Level * 100 XP required
        if (XP >= Level * 100)
        {
            XP -= Level * 100;
            Level++;
            MaxHP += 5; 
            CurrentHP = MaxHP; // Heal on level up
        }
    }

    /// <summary>
    /// Returns moves that are valid according to the piece's geometric rules, 
    /// ignoring check/pin constraints.
    /// </summary>
    public abstract IEnumerable<Position> GetPseudoLegalMoves(Position from, Board board);
}
