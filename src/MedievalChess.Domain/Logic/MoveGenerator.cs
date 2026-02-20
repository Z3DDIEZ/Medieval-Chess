using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public class MoveGenerator
{
    public IEnumerable<Move> GetLegalMoves(Board board, PlayerColor turn, bool isAttritionMode = false)
    {
        // Materialize to List to avoid lazy evaluation issues during iteration
        var activePieces = board.GetActivePieces(turn).ToList();
        var legalMoves = new List<Move>();

        foreach (var piece in activePieces)
        {
            if (piece.Position == null) continue;

            // CRITICAL: Materialize to List - GetPseudoLegalMoves uses yield return (lazy).
            // IsLegal() temporarily modifies piece.Position during simulation,
            // which would corrupt the lazy iterator's state.
            var pseudoMoves = piece.GetPseudoLegalMoves(piece.Position.Value, board).ToList();
            
            foreach (var target in pseudoMoves)
            {
                if (IsLegal(board, piece, target, isAttritionMode))
                {
                    var capturedPiece = board.GetPieceAt(target);
                    legalMoves.Add(new Move(piece.Position.Value, target, piece, capturedPiece));
                }
            }
        }
        return legalMoves;
    }

    public bool IsLegal(Board board, Piece piece, Position to, bool isAttritionMode = false)
    {
        if (piece.Position == null) return false;
        
        var from = piece.Position.Value;
        
        // 1. Check pseudo-legality first - the piece must be able to make this move
        var pseudoMoves = piece.GetPseudoLegalMoves(from, board).ToList();
        if (!pseudoMoves.Contains(to)) return false;
        
        // 2. Special castling validation
        if (piece.Type == PieceType.King && Math.Abs(to.File - from.File) == 2)
        {
            // Can't castle out of check
            if (!isAttritionMode && IsKingInCheck(board, piece.Color)) return false;
            
            // Can't castle through check - check intermediate square
            int direction = to.File > from.File ? 1 : -1;
            var intermediateSquare = new Position(from.File + direction, from.Rank);
            
            // Simulate king on intermediate square
            var originalPos = piece.Position;
            try
            {
                piece.Position = intermediateSquare;
                if (!isAttritionMode && IsKingInCheck(board, piece.Color)) return false;
            }
            finally
            {
                piece.Position = originalPos;
            }
        }
        
        // In Attrition Mode, checks do not invalidate moves
        if (isAttritionMode) return true;

        // 3. Simulate move with guaranteed state restoration
        var targetPiece = board.GetPieceAt(to);
        
        // Special handling for en passant
        Piece? enPassantCapture = null;
        if (piece.Type == PieceType.Pawn && 
            from.File != to.File && 
            targetPiece == null && 
            board.EnPassantTarget == to)
        {
            enPassantCapture = board.GetPieceAt(new Position(to.File, from.Rank));
        }
        
        try
        {
            // Apply simulation
            piece.Position = to;
            if (targetPiece != null) targetPiece.Position = null;
            if (enPassantCapture != null) enPassantCapture.Position = null;

            // Check if this leaves king in check
            return !IsKingInCheck(board, piece.Color);
        }
        finally
        {
            // Guaranteed revert - always restore state
            piece.Position = from;
            if (targetPiece != null) targetPiece.Position = to;
            if (enPassantCapture != null) enPassantCapture.Position = new Position(to.File, from.Rank);
        }
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
