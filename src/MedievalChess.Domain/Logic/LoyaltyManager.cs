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

        // 1. Identify Lords and Vassals
        var queens = pieces.Where(p => p.Type == PieceType.Queen).ToList();
        
        // 2. Apply passive modifiers
        foreach (var piece in pieces)
        {
            // +5 if adjacent to any Lord (Queen/King/Bishop/Rook) of same color
            bool adjacentToLord = pieces.Any(lord => 
                lord.Color == piece.Color && 
                lord != piece &&
                IsLord(lord.Type) &&
                lord.Position.HasValue && piece.Position.HasValue &&
                lord.Position.Value.IsAdjacentTo(piece.Position.Value)
            );

            if (adjacentToLord)
            {
                ModifyLoyalty(piece, 5);
            }
        }
    }

    /// <summary>
    /// To be called when a piece is captured.
    /// </summary>
    public void OnPieceCaptured(Piece capturedPiece)
    {
        // If a Lord is captured, their vassals panic (-30 LV)
        if (IsLord(capturedPiece.Type))
        {
            var vassals = GetVassalsFor(capturedPiece);
            foreach (var vassal in vassals)
            {
                ModifyLoyalty(vassal, -30);
            }
        }
    }

    private void ModifyLoyalty(Piece piece, int amount)
    {
        // Value Object logic is encapsulated, we need to create new one or update it
        int newValue = Math.Clamp(piece.Loyalty.Value + amount, 0, 100);
        piece.Loyalty = new LoyaltyValue(newValue);
    }

    private bool IsLord(PieceType type) => type switch
    {
        PieceType.King => true,
        PieceType.Queen => true,
        PieceType.Bishop => true,
        PieceType.Rook => true,
        _ => false
    };

    /// <summary>
    /// Returns the direct vassals of a Lord based on the Ruleset hierarchy.
    /// </summary>
    private IEnumerable<Piece> GetVassalsFor(Piece lord)
    {
        var allPieces = _game.Board.Pieces.Where(p => !p.IsCaptured && p.Color == lord.Color).ToList();

        switch (lord.Type)
        {
            case PieceType.King:
                // King's vassal is Queen
                return allPieces.Where(p => p.Type == PieceType.Queen);
            
            case PieceType.Queen:
                // Queen commands Bishops and Rooks
                return allPieces.Where(p => p.Type == PieceType.Bishop || p.Type == PieceType.Rook);
            
            case PieceType.Bishop:
            case PieceType.Rook:
                // They command Knights and Pawns in their sector
                // Simplified: All Pawns for now
                return allPieces.Where(p => p.Type == PieceType.Pawn || p.Type == PieceType.Knight);
                
            default:
                return Enumerable.Empty<Piece>();
        }
    }
}
