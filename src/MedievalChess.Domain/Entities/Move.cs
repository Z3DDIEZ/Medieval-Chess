using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Entities;

public class Move
{
    public Position From { get; }
    public Position To { get; }
    public Piece Piece { get; }
    public Piece? CapturedPiece { get; }
    
    // Special Move Flags
    public bool IsCastling { get; set; }
    public bool IsKingsideCastle { get; set; }
    public bool IsEnPassant { get; set; }
    public PieceType? PromotionPiece { get; set; }
    
    // Game State Flags
    public bool IsCapture => CapturedPiece != null || IsEnPassant;
    public bool IsCheck { get; set; }
    public bool IsCheckmate { get; set; }
    
    // Attrition Flags
    public int? DamageDealt { get; set; }
    public bool IsAttackBounce { get; set; }
    
    // Notation for UI/History
    public string Notation { get; set; } = string.Empty;

    public Move(Position from, Position to, Piece piece, Piece? capturedPiece = null)
    {
        From = from;
        To = to;
        Piece = piece;
        CapturedPiece = capturedPiece;
    }

    /// <summary>
    /// Generate standard algebraic notation for this move
    /// </summary>
    public string ToAlgebraicNotation()
    {
        // Castling
        if (IsCastling)
        {
            return IsKingsideCastle ? "O-O" : "O-O-O";
        }

        var sb = new System.Text.StringBuilder();

        // Piece letter (except pawn)
        if (Piece.Type != PieceType.Pawn)
        {
            sb.Append(GetPieceLetter(Piece.Type));
        }

        // Source disambiguation (simplified - just file for pawns on capture)
        if (Piece.Type == PieceType.Pawn && IsCapture)
        {
            sb.Append((char)('a' + From.File));
        }

        // Capture indicator
        if (IsCapture)
        {
            sb.Append('x');
        }

        // Destination square
        sb.Append(To.ToAlgebraic());

        // Promotion
        if (PromotionPiece.HasValue)
        {
            sb.Append('=');
            sb.Append(GetPieceLetter(PromotionPiece.Value));
        }

        // Check/Checkmate
        if (IsCheckmate) sb.Append('#');
        else if (IsCheck) sb.Append('+');

        // Attrition Bounce Indicator
        if (IsAttackBounce)
        {
            sb.Append('*'); // Asterisk indicates a hit that didn't kill/move
        }

        return sb.ToString();
    }

    private static char GetPieceLetter(PieceType type)
    {
        return type switch
        {
            PieceType.King => 'K',
            PieceType.Queen => 'Q',
            PieceType.Rook => 'R',
            PieceType.Bishop => 'B',
            PieceType.Knight => 'N',
            _ => ' '
        };
    }
}
