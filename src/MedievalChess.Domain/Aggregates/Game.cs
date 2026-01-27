using MedievalChess.Domain.Common;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Aggregates;

public class Game : AggregateRoot<Guid>
{
    public Board Board { get; private set; }
    public PlayerColor CurrentTurn { get; private set; }
    public GameStatus Status { get; private set; }
    public int TurnNumber { get; private set; }
    
    private readonly List<Move> _playedMoves = new();
    public IReadOnlyCollection<Move> PlayedMoves => _playedMoves.AsReadOnly();

    public PlayerColor? DrawOfferedBy { get; private set; }

    private Game() 
    {
        Board = null!; // EF Core binding
    }

    public static Game StartNew()
    {
        return new Game
        {
            Id = Guid.NewGuid(),
            Board = Board.CreateStandardSetup(),
            CurrentTurn = PlayerColor.White,
            Status = GameStatus.InProgress,
            TurnNumber = 1
        };
    }

    public void ExecuteMove(Position from, Position to, Logic.IEngineService engine, PieceType? promotionPiece = null)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is not in progress");

        // 1. Validate Legal Move (Engine)
        if (!engine.IsMoveLegal(Board, from, to, CurrentTurn))
        {
            throw new InvalidOperationException("Illegal move");
        }
        
        var piece = Board.GetPieceAt(from); 
        if (piece == null) throw new InvalidOperationException("System Error: Piece vanished during validation");

        var targetPiece = Board.GetPieceAt(to);
        
        // Create Move record
        var move = new Move(from, to, piece, targetPiece);

        // --- Detect Special Moves ---
        
        // Castling detection (King moves 2 squares)
        bool isCastling = piece.Type == PieceType.King && Math.Abs(to.File - from.File) == 2;
        if (isCastling)
        {
            move.IsCastling = true;
            move.IsKingsideCastle = to.File > from.File;
            
            // Move the rook as well
            int rookFromFile = move.IsKingsideCastle ? 7 : 0;
            int rookToFile = move.IsKingsideCastle ? 5 : 3;
            var rookFrom = new Position(rookFromFile, from.Rank);
            var rookTo = new Position(rookToFile, from.Rank);
            
            var rook = Board.GetPieceAt(rookFrom);
            if (rook != null)
            {
                rook.MoveTo(rookTo);
                rook.MarkAsMoved();
            }
        }
        
        // En passant detection (Pawn captures diagonally to empty square)
        bool isEnPassant = piece.Type == PieceType.Pawn && 
                           from.File != to.File && 
                           targetPiece == null;
        if (isEnPassant && Board.EnPassantTarget == to)
        {
            move.IsEnPassant = true;
            
            // Capture the pawn that made the double move (on same rank as moving pawn, target file)
            var capturedPawnPos = new Position(to.File, from.Rank);
            var capturedPawn = Board.GetPieceAt(capturedPawnPos);
            if (capturedPawn != null)
            {
                capturedPawn.Capture();
                Board.ResetHalfMoveClock();
            }
        }
        
        // Standard capture
        if (targetPiece != null)
        {
            targetPiece.Capture(); // Use proper capture instead of TakeDamage(999)
            Board.ResetHalfMoveClock();
        }
        
        // Execute the move
        piece.MoveTo(to);
        piece.MarkAsMoved();
        
        // Pawn promotion
        if (piece.Type == PieceType.Pawn && Entities.Pieces.Pawn.IsPromotionRank(to.Rank, piece.Color))
        {
            // Default to Queen if not specified
            var promoteToType = promotionPiece ?? PieceType.Queen;
            move.PromotionPiece = promoteToType;
            
            // Actually replace the pawn with the promoted piece
            piece.Capture(); // Remove the pawn from the board
            
            Piece promotedPiece = promoteToType switch
            {
                PieceType.Queen => new Entities.Pieces.Queen(piece.Color, to),
                PieceType.Rook => new Entities.Pieces.Rook(piece.Color, to),
                PieceType.Bishop => new Entities.Pieces.Bishop(piece.Color, to),
                PieceType.Knight => new Entities.Pieces.Knight(piece.Color, to),
                _ => new Entities.Pieces.Queen(piece.Color, to)
            };
            promotedPiece.MarkAsMoved();
            Board.AddPiece(promotedPiece);
        }
        
        // --- Update Board State ---
        
        // Update castling rights
        Board.UpdateCastlingRights(piece, from);
        
        // Set en passant target (only if pawn moved 2 squares)
        if (piece.Type == PieceType.Pawn && Math.Abs(to.Rank - from.Rank) == 2)
        {
            int epRank = (from.Rank + to.Rank) / 2;
            Board.SetEnPassantTarget(new Position(from.File, epRank));
        }
        else
        {
            Board.SetEnPassantTarget(null);
        }
        
        // Update half-move clock (pawn moves reset it)
        if (piece.Type == PieceType.Pawn)
        {
            Board.ResetHalfMoveClock();
        }
        else if (targetPiece == null && !isEnPassant)
        {
            Board.IncrementHalfMoveClock();
        }
        
        // Generate proper notation
        move.Notation = move.ToAlgebraicNotation();
        _playedMoves.Add(move);

        EndTurn(engine);
    }
    
    private void EndTurn(Logic.IEngineService engine)
    {
        CurrentTurn = CurrentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        if (CurrentTurn == PlayerColor.White)
        {
            TurnNumber++;
        }
        
        // Set check/checkmate flags on the last move
        if (_playedMoves.Count > 0)
        {
            var lastMove = _playedMoves[^1];
            if (engine.IsKingInCheck(Board, CurrentTurn))
            {
                lastMove.IsCheck = true;
                
                if (engine.IsCheckmate(Board, CurrentTurn))
                {
                    lastMove.IsCheckmate = true;
                    lastMove.Notation = lastMove.ToAlgebraicNotation(); // Re-generate with checkmate symbol
                    Status = GameStatus.Checkmate;
                    return;
                }
            }
        }
        
        // Check for stalemate
        if (engine.IsStalemate(Board, CurrentTurn))
        {
            Status = GameStatus.Stalemate;
            return;
        }
        
        // Check for 50-move rule
        if (Board.IsFiftyMoveRule)
        {
            Status = GameStatus.Draw;
        }
    }

    public void Resign(PlayerColor player)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is not in progress");

        Status = GameStatus.Resignation;
        // Logic for winner would go here
    }

    public void MakeDrawOffer(PlayerColor player)
    {
        if (Status != GameStatus.InProgress) return;
        if (DrawOfferedBy == player) return; // Already offered
        
        if (DrawOfferedBy.HasValue && DrawOfferedBy != player)
        {
            // Both offered -> Draw
            Status = GameStatus.Draw;
            DrawOfferedBy = null;
        }
        else
        {
            DrawOfferedBy = player;
        }
    }

    public void AcceptDraw(PlayerColor player)
    {
        if (Status != GameStatus.InProgress) return;
        if (DrawOfferedBy.HasValue && DrawOfferedBy != player)
        {
            Status = GameStatus.Draw;
            DrawOfferedBy = null;
        }
    }
}
