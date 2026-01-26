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
        var pawnStart = new Position(4, 1); // e2
        var pawnTarget = new Position(4, 3); // e4

        game.ExecuteMove(pawnStart, pawnTarget);

        Assert.Equal(PlayerColor.Black, game.CurrentTurn);
        // Turn number only increments after black plays (Move 1 is White+Black)
        // Or traditionally, "Turn 1" covers both. 
        // Our logic: "CurrentTurn == White ? TurnNumber++" on EndTurn.
        // Post White move -> Turn is Black. TurnNumber still 1.
        Assert.Equal(1, game.TurnNumber); 
        
        // Execute Black move
        var blackPawn = new Position(4, 6); // e7
        var blackTarget = new Position(4, 4); // e5
        game.ExecuteMove(blackPawn, blackTarget);

        Assert.Equal(PlayerColor.White, game.CurrentTurn);
        Assert.Equal(2, game.TurnNumber);
    }
}
