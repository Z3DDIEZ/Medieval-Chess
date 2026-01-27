using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public class LoyaltyManager
{
    private readonly Game _game;

    public LoyaltyManager(Game game)
    {
        _game = game;
    }

    /// <summary>
    /// Recalculates loyalty for all pieces based on current board state.
    /// Should be called at start of turn or after major events.
    /// </summary>
    public void UpdateLoyalty()
    {
        var pieces = _game.Board.Pieces.Where(p => !p.IsCaptured).ToList();

        foreach (var piece in pieces)
        {
            // 1. Find Lord
            var lord = GetLordOf(piece);

            // 2. Apply passive modifiers
            if (lord != null)
            {
                // +5 if adjacent to YOUR Lord
                bool adjacentToLord = lord.Position.HasValue && 
                                      piece.Position.HasValue && 
                                      lord.Position.Value.IsAdjacentTo(piece.Position.Value);
                if (adjacentToLord)
                {
                    ModifyLoyalty(piece, 5);
                }
            }
            
            // Note: "Piece takes damage without nearby allies" would be handled in TakeDamage event or here if we track damage state
        }
    }

    /// <summary>
    /// To be called when a piece is captured.
    /// </summary>
    public void OnPieceCaptured(Piece capturedPiece)
    {
        // If a Lord is captured, their vassals panic (-30 LV)
        // We check if this piece IS a lord to anyone
        var vassals = GetVassalsOf(capturedPiece);
        foreach (var vassal in vassals)
        {
            ModifyLoyalty(vassal, -30);
            // In a full implementation, we might mark them as Orphaned here
        }
    }

    private void ModifyLoyalty(Piece piece, int amount)
    {
        int newValue = Math.Clamp(piece.Loyalty.Value + amount, 0, 100);
        piece.Loyalty = new LoyaltyValue(newValue);
        
        // Also update the relationship record if we want to persist it there
        var relationship = _game.LoyaltyRelationships.FirstOrDefault(r => r.VassalId == piece.Id);
        relationship?.AdjustLoyalty(amount);
    }

    private Piece? GetLordOf(Piece vassal)
    {
        var rel = _game.LoyaltyRelationships.FirstOrDefault(r => r.VassalId == vassal.Id);
        if (rel == null) return null;
        return _game.Board.Pieces.FirstOrDefault(p => p.Id == rel.LordId);
    }

    private IEnumerable<Piece> GetVassalsOf(Piece lord)
    {
        var vassalIds = _game.LoyaltyRelationships
            .Where(r => r.LordId == lord.Id)
            .Select(r => r.VassalId)
            .ToList();
            
        return _game.Board.Pieces.Where(p => vassalIds.Contains(p.Id));
    }
}
