using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public class EngineService : IEngineService
{
    private readonly MoveGenerator _moveGenerator = new();

    public bool IsMoveLegal(Board board, Position from, Position to, PlayerColor turn)
    {
        var piece = board.GetPieceAt(from);
        if (piece == null || piece.Color != turn) return false;

        return _moveGenerator.IsLegal(board, piece, to);
    }

    public bool IsKingInCheck(Board board, PlayerColor color)
    {
        return _moveGenerator.IsKingInCheck(board, color);
    }

    public bool IsCheckmate(Board board, PlayerColor color)
    {
        if (!IsKingInCheck(board, color)) return false;
        
        var legalMoves = _moveGenerator.GetLegalMoves(board, color);
        return !legalMoves.Any();
    }

    public bool IsStalemate(Board board, PlayerColor color)
    {
        if (IsKingInCheck(board, color)) return false;
        
        var legalMoves = _moveGenerator.GetLegalMoves(board, color);
        return !legalMoves.Any();
    }
    
    // Explicit Move Generation if needed by interface, but currently interface only asks for booleans mostly.
    // Except Game might need list of moves for UI? 
    // The previous implementation had GetLegalDestinations.
    
    public IEnumerable<Position> GetLegalDestinations(Board board, Position from)
    {
        var piece = board.GetPieceAt(from);
        if (piece == null) return Enumerable.Empty<Position>();

        var moves = _moveGenerator.GetLegalMoves(board, piece.Color); // This generates ALL moves for the color.
        // Optimization: MoveGenerator could have GetLegalMoves(Piece).
        // For now, filter:
        return moves.Where(m => m.From == from).Select(m => m.To);
    }
}
