using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public class Move
{
    public Position From { get; }
    public Position To { get; }
    public Piece Piece { get; }
    public Piece? CapturedPiece { get; }
    public bool IsCheck { get; set; }
    public bool IsCheckmate { get; set; }
    
    // Additional metadata for UI/History
    public string Notation { get; set; } = string.Empty;

    public Move(Position from, Position to, Piece piece, Piece? capturedPiece = null)
    {
        From = from;
        To = to;
        Piece = piece;
        CapturedPiece = capturedPiece;
    }
}
