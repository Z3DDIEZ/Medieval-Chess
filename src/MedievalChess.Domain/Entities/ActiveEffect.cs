using MedievalChess.Domain.Common;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Entities;

public class ActiveEffect : Entity<Guid>
{
    public Guid PieceId { get; private set; }
    public EffectType Type { get; private set; }
    public int Magnitude { get; private set; }
    public int RemainingDuration { get; private set; } // In turns
    public Guid? SourceAbilityId { get; private set; }

    private ActiveEffect() { } // EF Core

    public ActiveEffect(Guid pieceId, EffectType type, int magnitude, int duration, Guid? sourceAbilityId = null)
    {
        Id = Guid.NewGuid();
        PieceId = pieceId;
        Type = type;
        Magnitude = magnitude;
        RemainingDuration = duration;
        SourceAbilityId = sourceAbilityId;
    }

    public void DecrementDuration()
    {
        RemainingDuration = Math.Max(0, RemainingDuration - 1);
    }

    public bool IsExpired => RemainingDuration <= 0;
}
