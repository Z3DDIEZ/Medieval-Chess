using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Tests;

public class GameTests
{
    [Fact]
    public void StartNew_ShouldInitializeCorrectly()
    {
        var game = Game.StartNew();

        Assert.NotNull(game.Board);
        Assert.Equal(PlayerColor.White, game.CurrentTurn);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(1, game.TurnNumber);
        Assert.NotEmpty(game.Board.Pieces);
    }

    [Fact]
    public void ExecuteMove_ShouldSwitchTurn()
    {
        var game = Game.StartNew();
        var engine = new MedievalChess.Domain.Logic.EngineService();
        var pawnStart = new Position(4, 1); // e2
        var pawnTarget = new Position(4, 3); // e4

        game.ExecuteMove(pawnStart, pawnTarget, engine);

        Assert.Equal(PlayerColor.Black, game.CurrentTurn);
        Assert.Equal(1, game.TurnNumber); 
        
        // Execute Black move
        var blackPawn = new Position(4, 6); // e7
        var blackTarget = new Position(4, 4); // e5
        game.ExecuteMove(blackPawn, blackTarget, engine);

        Assert.Equal(PlayerColor.White, game.CurrentTurn);
        Assert.Equal(2, game.TurnNumber);
    }
}
