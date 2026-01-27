using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public class MoveGenerator
{
    public IEnumerable<Move> GetLegalMoves(Board board, PlayerColor turn)
    {
        var activePieces = board.GetActivePieces(turn);
        var legalMoves = new List<Move>();

        foreach (var piece in activePieces)
        {
            if (piece.Position == null) continue;

            var pseudoMoves = piece.GetPseudoLegalMoves(piece.Position.Value, board);
            foreach (var target in pseudoMoves)
            {
                if (IsLegal(board, piece, target))
                {
                    var capturedPiece = board.GetPieceAt(target);
                    legalMoves.Add(new Move(piece.Position.Value, target, piece, capturedPiece));
                }
            }
        }
        return legalMoves;
    }

    public bool IsLegal(Board board, Piece piece, Position to)
    {
        // Simulate move
        var from = piece.Position!.Value;
        var targetPiece = board.GetPieceAt(to);
        
        // Apply
        piece.Position = to;
        if (targetPiece != null) targetPiece.Position = null;

        var kingInCheck = IsKingInCheck(board, piece.Color);

        // Revert
        piece.Position = from;
        if (targetPiece != null) targetPiece.Position = to; // Restore captured piece position

        return !kingInCheck;
    }

    public bool IsKingInCheck(Board board, PlayerColor color)
    {
        var king = board.GetKing(color);
        if (king == null || king.Position == null) return true; // Should not happen in normal play

        // Check if any enemy piece attacks the king
        var enemyColor = color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        var enemies = board.GetActivePieces(enemyColor);

        foreach (var enemy in enemies)
        {
            if (enemy.Position == null) continue;
            
            // Optimization: Filter obvious non-attackers?
            // For now, check if enemy CanAttack(KingPos)
            // We use GetPseudoLegalMoves. 
            // Note: Pawn attacks are different from moves. Pawn.GetPseudoLegal includes attacks only if occupied.
            // But we want to know if it *threatens* the square, even if empty?
            // "Attacked" means an enemy PIECE could move there (capture king).
            // Current GetPseudoLegal for Pawn only returns capture moves IF occupied. 
            // So if King is there, it is occupied, so it works.
            
            var moves = enemy.GetPseudoLegalMoves(enemy.Position.Value, board);
            if (moves.Contains(king.Position.Value))
            {
                return true;
            }
        }
        return false;
    }
}
