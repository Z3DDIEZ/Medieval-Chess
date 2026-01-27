using MedievalChess.Domain.Common;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Entities;

public class PieceAbility : Entity<Guid>
{
    public Guid PieceId { get; private set; }
    public Guid AbilityDefinitionId { get; private set; } // Reference to static data
    public int UpgradeTier { get; private set; }
    public int CurrentCooldown { get; private set; }
    public int MaxCooldown { get; private set; }

    private PieceAbility() { }

    public PieceAbility(Guid pieceId, Guid abilityDefinitionId, int maxCooldown)
    {
        Id = Guid.NewGuid();
        PieceId = pieceId;
        AbilityDefinitionId = abilityDefinitionId;
        MaxCooldown = maxCooldown;
        CurrentCooldown = 0;
        UpgradeTier = 0;
    }

    public void Upgrade()
    {
        if (UpgradeTier < 3)
        {
            UpgradeTier++;
        }
    }

    public void TriggerCooldown()
    {
        CurrentCooldown = MaxCooldown;
    }

    public void ReduceCooldown(int amount = 1)
    {
        CurrentCooldown = Math.Max(0, CurrentCooldown - amount);
    }

    public void ResetCooldown()
    {
        CurrentCooldown = 0;
    }
    
    public bool IsReady => CurrentCooldown <= 0;
}
