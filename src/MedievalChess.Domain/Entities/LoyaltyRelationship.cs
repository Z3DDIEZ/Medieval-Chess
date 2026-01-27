using MedievalChess.Domain.Common;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public class LoyaltyRelationship : Entity<Guid>
{
    public Guid VassalId { get; private set; }
    public Guid LordId { get; private set; }
    public LoyaltyValue Loyalty { get; private set; }
    public DateTime EstablishedAt { get; private set; }
    public DateTime LastModifiedAt { get; private set; }

    private LoyaltyRelationship() { } // EF Core

    public LoyaltyRelationship(Guid vassalId, Guid lordId, int initialLoyalty = 70)
    {
        Id = Guid.NewGuid();
        VassalId = vassalId;
        LordId = lordId;
        Loyalty = new LoyaltyValue(initialLoyalty);
        EstablishedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AdjustLoyalty(int delta)
    {
        Loyalty = Loyalty.Adjust(delta);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void SetLord(Guid newLordId)
    {
        LordId = newLordId;
        LastModifiedAt = DateTime.UtcNow;
    }
}
