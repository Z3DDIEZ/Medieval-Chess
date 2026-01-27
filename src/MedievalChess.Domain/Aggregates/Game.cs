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

    public void ExecuteMove(Position from, Position to, Logic.IEngineService engine)
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
        
        // Record Move
        var move = new Move(from, to, piece, targetPiece);
        move.Notation = $"{piece.Type.ToString()[0]}{to.ToAlgebraic()}"; // Simplified notation (e.g. "Pd4", "Nf3") - TODO: Full algebraic
        _playedMoves.Add(move);

        if (targetPiece != null)
        {
            // Capture logic (Standard Chess / Medieval Attrition base)
            // For now, Standard Chess: Capture = Remove (Capture happens if HP hits 0, but standard chess HP is effectively 1 or manual logic)
            // The existing code had TakeDamage(999).
            
            targetPiece.TakeDamage(999); 
            
            var loyaltyManager = new Logic.LoyaltyManager(this);
            loyaltyManager.OnPieceCaptured(targetPiece);
        }

        piece.MoveTo(to);
        
        var loyaltyUpdate = new Logic.LoyaltyManager(this);
        loyaltyUpdate.UpdateLoyalty();

        EndTurn(engine);
    }
    
    private void EndTurn(Logic.IEngineService engine)
    {
        CurrentTurn = CurrentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        if (CurrentTurn == PlayerColor.White)
        {
            TurnNumber++;
        }
        
        // Update Game Status (Checkmate/Stalemate)
        if (engine.IsCheckmate(Board, CurrentTurn))
        {
            Status = GameStatus.Checkmate;
        }
        else if (engine.IsStalemate(Board, CurrentTurn))
        {
            Status = GameStatus.Stalemate;
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
