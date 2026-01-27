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
        // Note: piece is guaranteed !null and Correct Color by IsMoveLegal, but strict null check is fine
        if (piece == null) throw new InvalidOperationException("System Error: Piece vanished during validation");

        var targetPiece = Board.GetPieceAt(to);
        if (targetPiece != null)
        {
            // Capture logic
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
        // Winner is the other player
        // We might want to store Winner property, but for now Status implies it if we know who resigned.
        // Actually, usually "WhiteResigned" or "BlackResigned" is better, or just store Winner.
        // For simplicity, let's assume the caller handles the "Winner" logic or we add a Winner property later.
        // But wait, if I resign, the game status is Resignation. Who won?
        // Let's add specific statuses or a Winner field. 
        // For this task, "Resignation" is enough, we can infer winner from who sent the command if we tracked it.
        // But the Game entity doesn't track "who sent the command" in the state persistence usually.
        // Let's keep it simple: generic Resignation.
    }

    public void OfferDraw(PlayerColor player)
    {
        // For now, simple mechanic: if one offers, and we had state for "DrawOfferedByWhite", etc.
        // Since we don't have that field, let's just assume this method *completes* a draw if agreed?
        // No, typically: P1 offers -> State: DrawOffered -> P2 accepts -> State: Draw.
        // I need to add a DrawOfferedBy property to Game.
    }
    
    // Simplification for prototype: Immediate Draw (Mutual Agreement handled by UI co-ordination or just a button that says 'Declare Draw')
    // Better: Add "DrawOfferedBy" property.
    
    public PlayerColor? DrawOfferedBy { get; private set; }

    public void MakeDrawOffer(PlayerColor player)
    {
        if (Status != GameStatus.InProgress) return;
        if (DrawOfferedBy == player) return; // Already offered
        
        if (DrawOfferedBy.HasValue && DrawOfferedBy != player)
        {
            // Both offered? -> Draw
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

    private void EndTurn()
    {
        CurrentTurn = CurrentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        if (CurrentTurn == PlayerColor.White)
        {
            TurnNumber++;
        }
        // Draw offer expires on turn end? Usually yes or no depending on rules. Let's keep it for a turn.
        // basic rules: offer remains valid until rejected or moved? 
        // keeping it simple: offer persists until accepted or game ends.
    }
}
