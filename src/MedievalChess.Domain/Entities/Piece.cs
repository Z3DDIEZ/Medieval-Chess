using MedievalChess.Domain.Common;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public class Piece : AggregateRoot<Guid>
{
    public PieceType Type { get; private set; }
    public PlayerColor Color { get; private set; }
    public Position? Position { get; private set; } // Null if captured
    public LoyaltyValue Loyalty { get; internal set; }
    
    // Progression
    public int Level { get; private set; }
    public int XP { get; private set; }
    
    // Combat (Attrition Mode)
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    
    public bool IsCaptured => Position == null;

    // Factory method for creation
    public static Piece Create(PieceType type, PlayerColor color, Position position)
    {
        var piece = new Piece
        {
            Id = Guid.NewGuid(),
            Type = type,
            Color = color,
            Position = position,
            Loyalty = new LoyaltyValue(80), // Default Loyal
            Level = 1,
            XP = 0,
            MaxHP = CalculateBaseHP(type),
            CurrentHP = CalculateBaseHP(type)
        };
        return piece;
    }

    private Piece() { } // EF Core requirement

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

    private static int CalculateBaseHP(PieceType type) => type switch
    {
        PieceType.Pawn => 20,
        PieceType.Knight => 40,
        PieceType.Bishop => 40,
        PieceType.Rook => 60,
        PieceType.Queen => 80,
        PieceType.King => 100,
        _ => 20
    };
}
