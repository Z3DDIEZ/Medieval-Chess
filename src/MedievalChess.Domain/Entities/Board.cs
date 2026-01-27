using MedievalChess.Domain.Entities.Pieces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public class Board
{
    private readonly List<Piece> _pieces = new();
    public IReadOnlyCollection<Piece> Pieces => _pieces.AsReadOnly();

    // Chess State Tracking
    public Position? EnPassantTarget { get; private set; }
    
    // Castling Rights
    public bool WhiteKingsideCastle { get; private set; } = true;
    public bool WhiteQueensideCastle { get; private set; } = true;
    public bool BlackKingsideCastle { get; private set; } = true;
    public bool BlackQueensideCastle { get; private set; } = true;
    
    // Draw tracking
    public int HalfMoveClock { get; private set; } // For 50-move rule

    public Board() { }

    public static Board CreateStandardSetup()
    {
        var board = new Board();
        
        // Add Pawns
        for (int file = 0; file < 8; file++)
        {
            board.AddPiece(new Pawn(PlayerColor.White, new Position(file, 1)));
            board.AddPiece(new Pawn(PlayerColor.Black, new Position(file, 6)));
        }

        // Add Pieces
        // White Back Rank (Rank 0)
        board.AddPiece(new Rook(PlayerColor.White, new Position(0, 0)));
        board.AddPiece(new Knight(PlayerColor.White, new Position(1, 0)));
        board.AddPiece(new Bishop(PlayerColor.White, new Position(2, 0)));
        board.AddPiece(new Queen(PlayerColor.White, new Position(3, 0)));
        board.AddPiece(new King(PlayerColor.White, new Position(4, 0)));
        board.AddPiece(new Bishop(PlayerColor.White, new Position(5, 0)));
        board.AddPiece(new Knight(PlayerColor.White, new Position(6, 0)));
        board.AddPiece(new Rook(PlayerColor.White, new Position(7, 0)));

        // Black Back Rank (Rank 7)
        board.AddPiece(new Rook(PlayerColor.Black, new Position(0, 7)));
        board.AddPiece(new Knight(PlayerColor.Black, new Position(1, 7)));
        board.AddPiece(new Bishop(PlayerColor.Black, new Position(2, 7)));
        board.AddPiece(new Queen(PlayerColor.Black, new Position(3, 7)));
        board.AddPiece(new King(PlayerColor.Black, new Position(4, 7)));
        board.AddPiece(new Bishop(PlayerColor.Black, new Position(5, 7)));
        board.AddPiece(new Knight(PlayerColor.Black, new Position(6, 7)));
        board.AddPiece(new Rook(PlayerColor.Black, new Position(7, 7)));

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

    // --- Board State Management ---
    
    /// <summary>
    /// Properly capture a piece (for standard chess mode - no damage system)
    /// </summary>
    public void CapturePiece(Piece piece)
    {
        piece.Position = null; // Mark as captured
    }

    /// <summary>
    /// Set or clear the en passant target square
    /// </summary>
    public void SetEnPassantTarget(Position? target)
    {
        EnPassantTarget = target;
    }

    /// <summary>
    /// Update castling rights based on piece movement
    /// </summary>
    public void UpdateCastlingRights(Piece movedPiece, Position from)
    {
        // King moves - lose all castling rights for that color
        if (movedPiece.Type == PieceType.King)
        {
            if (movedPiece.Color == PlayerColor.White)
            {
                WhiteKingsideCastle = false;
                WhiteQueensideCastle = false;
            }
            else
            {
                BlackKingsideCastle = false;
                BlackQueensideCastle = false;
            }
        }
        
        // Rook moves - lose castling right for that side
        if (movedPiece.Type == PieceType.Rook)
        {
            if (movedPiece.Color == PlayerColor.White)
            {
                if (from.File == 0 && from.Rank == 0) WhiteQueensideCastle = false;
                if (from.File == 7 && from.Rank == 0) WhiteKingsideCastle = false;
            }
            else
            {
                if (from.File == 0 && from.Rank == 7) BlackQueensideCastle = false;
                if (from.File == 7 && from.Rank == 7) BlackKingsideCastle = false;
            }
        }
    }

    /// <summary>
    /// Check if a specific castling is available
    /// </summary>
    public bool CanCastle(PlayerColor color, bool kingside)
    {
        if (color == PlayerColor.White)
            return kingside ? WhiteKingsideCastle : WhiteQueensideCastle;
        else
            return kingside ? BlackKingsideCastle : BlackQueensideCastle;
    }

    /// <summary>
    /// Increment half-move clock (reset on pawn move or capture)
    /// </summary>
    public void IncrementHalfMoveClock()
    {
        HalfMoveClock++;
    }

    public void ResetHalfMoveClock()
    {
        HalfMoveClock = 0;
    }

    /// <summary>
    /// Check if 50-move rule applies
    /// </summary>
    public bool IsFiftyMoveRule => HalfMoveClock >= 100; // 100 half-moves = 50 full moves
}
