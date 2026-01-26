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

    private Game() 
    {
        Board = null!; // EF Core binding
    }

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

    public void ExecuteMove(Position from, Position to)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is not in progress");

        var piece = Board.GetPieceAt(from);
        if (piece == null)
            throw new ArgumentException("No piece at source position");

        if (piece.Color != CurrentTurn)
            throw new InvalidOperationException("Not your turn");

        // Basic validation logic placeholder
        // TODO: Integrate Engine Validation here

        var targetPiece = Board.GetPieceAt(to);
        if (targetPiece != null)
        {
            if (targetPiece.Color == CurrentTurn)
                throw new InvalidOperationException("Cannot capture own piece");
            
            // Capture logic
            targetPiece.TakeDamage(999); // Instant capture for Phase 1
        }

        piece.MoveTo(to);
        EndTurn();
    }

    private void EndTurn()
    {
        CurrentTurn = CurrentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        if (CurrentTurn == PlayerColor.White)
        {
            TurnNumber++;
        }
    }
}
