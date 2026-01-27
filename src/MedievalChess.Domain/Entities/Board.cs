using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public class Board
{
    private readonly List<Piece> _pieces = new();
    public IReadOnlyCollection<Piece> Pieces => _pieces.AsReadOnly();

    public Board() { }

    public static Board CreateStandardSetup()
    {
        var board = new Board();
        
        // Add Pawns
        for (int file = 0; file < 8; file++)
        {
            board.AddPiece(Piece.Create(PieceType.Pawn, PlayerColor.White, new Position(file, 1)));
            board.AddPiece(Piece.Create(PieceType.Pawn, PlayerColor.Black, new Position(file, 6)));
        }

        // Add Pieces (Simplified loop for now, just Rooks to Kings)
        // This would be expanded for full setup
        var backRankTypes = new[] 
        { 
            PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, 
            PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook 
        };

        for (int file = 0; file < 8; file++)
        {
            board.AddPiece(Piece.Create(backRankTypes[file], PlayerColor.White, new Position(file, 0)));
            board.AddPiece(Piece.Create(backRankTypes[file], PlayerColor.Black, new Position(file, 7)));
        }

        return board;
    }

    public void AddPiece(Piece piece)
    {
        _pieces.Add(piece);
    }

    public Piece? GetPieceAt(Position position)
    {
        return _pieces.FirstOrDefault(p => !p.IsCaptured && p.Position == position);
    }

    public bool IsOccupied(Position position) => GetPieceAt(position) != null;

    public bool IsOccupiedByEnemy(Position position, PlayerColor myColor)
    {
        var piece = GetPieceAt(position);
        return piece != null && piece.Color != myColor;
    }

    public bool IsOccupiedByFriend(Position position, PlayerColor myColor)
    {
        var piece = GetPieceAt(position);
        return piece != null && piece.Color == myColor;
    }

    public IEnumerable<Piece> GetActivePieces(PlayerColor color)
    {
        return _pieces.Where(p => !p.IsCaptured && p.Color == color);
    }
    
    public Piece? GetKing(PlayerColor color)
    {
        return _pieces.FirstOrDefault(p => !p.IsCaptured && p.Color == color && p.Type == PieceType.King);
    }
}
