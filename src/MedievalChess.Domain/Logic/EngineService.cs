using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public class EngineService : IEngineService
{
    public bool IsMoveLegal(Board board, Position from, Position to, PlayerColor turn)
    {
        var piece = board.GetPieceAt(from);
        if (piece == null || piece.Color != turn) return false;
        
        // 1. Psuedo-legal check (Geometry & Path)
        if (!IsPseudoLegal(board, piece, to)) return false;

        // 2. King Safety (Make-Unmake)
        // Store State
        var startPos = piece.Position!.Value;
        var targetPiece = board.GetPieceAt(to);
        var targetStartPos = targetPiece?.Position;

        // Apply
        piece.Position = to;
        if (targetPiece != null) targetPiece.Position = null; // Capture

        bool kingInCheck = IsKingInCheck(board, turn);

        // Revert
        piece.Position = startPos;
        if (targetPiece != null) targetPiece.Position = targetStartPos;

        return !kingInCheck;
    }

    public bool IsKingInCheck(Board board, PlayerColor color)
    {
        var king = board.GetKing(color);
        if (king == null || king.Position == null) return true; // Should ideally not happen unless King captured (lose condition)

        return IsSquareAttacked(board, king.Position.Value, color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White);
    }

    public bool IsCheckmate(Board board, PlayerColor color)
    {
        if (!IsKingInCheck(board, color)) return false;
        return !HasAnyLegalMove(board, color);
    }

    public bool IsStalemate(Board board, PlayerColor color)
    {
        if (IsKingInCheck(board, color)) return false;
        return !HasAnyLegalMove(board, color);
    }
    
    public IEnumerable<Position> GetLegalDestinations(Board board, Position from)
    {
        var piece = board.GetPieceAt(from);
        if (piece == null) return Enumerable.Empty<Position>();

        var pseudoMoves = GetPseudoLegalDestinations(board, piece);
        var legalMoves = new List<Position>();

        foreach (var to in pseudoMoves)
        {
            if (IsMoveLegal(board, from, to, piece.Color))
            {
                legalMoves.Add(to);
            }
        }
        return legalMoves;
    }

    private bool HasAnyLegalMove(Board board, PlayerColor color)
    {
        var pieces = board.GetActivePieces(color).ToList();
        foreach (var piece in pieces)
        {
            if (piece.Position == null) continue;
            var moves = GetLegalDestinations(board, piece.Position.Value);
            if (moves.Any()) return true;
        }
        return false;
    }

    private bool IsSquareAttacked(Board board, Position square, PlayerColor attackerColor)
    {
        var attackers = board.GetActivePieces(attackerColor);
        foreach (var attacker in attackers)
        {
            if (IsPseudoLegal(board, attacker, square, ignoreKingCheck: true))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPseudoLegal(Board board, Piece piece, Position to, bool ignoreKingCheck = false)
    {
        if (piece.Position == null) return false;
        if (piece.Position.Value == to) return false; // Cannot move to same square

        // Target check
        var target = board.GetPieceAt(to);
        if (target != null && target.Color == piece.Color) return false; // Cannot capture own piece

        var from = piece.Position.Value;
        int dx = to.File - from.File;
        int dy = to.Rank - from.Rank;
        int absDx = Math.Abs(dx);
        int absDy = Math.Abs(dy);

        switch (piece.Type)
        {
            case PieceType.Pawn:
                int direction = piece.Color == PlayerColor.White ? 1 : -1;
                // Move forward 1
                if (dx == 0 && dy == direction) return target == null;
                // Move forward 2
                if (dx == 0 && dy == 2 * direction) 
                {
                    if (target != null) return false;
                    // Must be on starting rank
                    int startRank = piece.Color == PlayerColor.White ? 1 : 6;
                    if (from.Rank != startRank) return false;
                    // Must have clear path
                    var midSquare = new Position(from.File, from.Rank + direction);
                    if (board.IsOccupied(midSquare)) return false;
                    return true;
                }
                // Capture
                if (absDx == 1 && dy == direction)
                {
                    // Regular capture
                    if (target != null && target.Color != piece.Color) return true;
                    // En Passant (TODO: Need EnPassantTarget in Game State)
                    return false; 
                }
                return false;

            case PieceType.Knight:
                return (absDx == 1 && absDy == 2) || (absDx == 2 && absDy == 1);

            case PieceType.Bishop:
                if (absDx != absDy) return false;
                return IsPathClear(board, from, to);

            case PieceType.Rook:
                if (dx != 0 && dy != 0) return false;
                return IsPathClear(board, from, to);

            case PieceType.Queen:
                if (absDx != absDy && (dx != 0 && dy != 0)) return false;
                return IsPathClear(board, from, to);

            case PieceType.King:
                // Normal move
                if (absDx <= 1 && absDy <= 1) return true;
                // Castling (TODO)
                return false;
                
            default:
                return false;
        }
    }

    private IEnumerable<Position> GetPseudoLegalDestinations(Board board, Piece piece)
    {
        var destinations = new List<Position>();
        // Iterate all squares? Inefficient.
        // Better: Project moves based on type.
        // For simplicity v1: Iterate all squares and check PsuedoLegal. (64 checks, fast enough)
        
        for (int r = 0; r < 8; r++)
        {
            for (int f = 0; f < 8; f++)
            {
                var target = new Position(f, r);
                if (IsPseudoLegal(board, piece, target))
                {
                    destinations.Add(target);
                }
            }
        }
        return destinations;
    }

    private bool IsPathClear(Board board, Position from, Position to)
    {
        int dx = Math.Sign(to.File - from.File);
        int dy = Math.Sign(to.Rank - from.Rank);
        
        var current = new Position(from.File + dx, from.Rank + dy);
        while (current != to)
        {
            if (board.IsOccupied(current)) return false;
            current = new Position(current.File + dx, current.Rank + dy);
        }
        return true;
    }
}
