using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Entities.Pieces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;
using Xunit;

namespace MedievalChess.Domain.Tests.Logic;

public class LoyaltyManagerTests
{
    private readonly Game _game;
    private readonly LoyaltyManager _loyaltyManager;

    public LoyaltyManagerTests()
    {
        _game = Game.StartNew();
        _loyaltyManager = new LoyaltyManager(_game);
    }

    [Fact]
    public void UpdateLoyalty_AdjacentToLord_IncreasesLoyalty()
    {
        // Arrange
        var pawnD2 = _game.Board.GetPieceAt(new Position(3, 1)); // White Pawn
        Assert.NotNull(pawnD2);
        var queen = _game.Board.GetPieceAt(new Position(3, 0)); // White Queen
        Assert.NotNull(queen);
        
        // Establish relationship
        _game.AddLoyaltyRelationship(new LoyaltyRelationship(pawnD2.Id, queen.Id));
        pawnD2.Loyalty = new LoyaltyValue(70);

        // Act
        _loyaltyManager.UpdateLoyalty();

        // Assert
        // Should gain +5 for being adjacent to Queen
        Assert.Equal(75, pawnD2.Loyalty.Value); 
    }

    [Fact]
    public void OnPieceCaptured_LordCaptured_ReducesVassalLoyalty()
    {
        // Arrange
        // Establish relationship: Queen is Lord of PawnD2
        var queen = _game.Board.GetPieceAt(new Position(3, 0)); // White Queen
        Assert.NotNull(queen);
        var pawnD2 = _game.Board.GetPieceAt(new Position(3, 1)); // White Pawn
        Assert.NotNull(pawnD2);
        
        _game.AddLoyaltyRelationship(new LoyaltyRelationship(pawnD2.Id, queen.Id));

        pawnD2.Loyalty = new LoyaltyValue(80);

        // Act
        _loyaltyManager.OnPieceCaptured(queen);

        // Assert
        Assert.Equal(50, pawnD2.Loyalty.Value);
    }

    [Fact]
    public void ProcessDefections_FlipsColorAndRemovesRelationships()
    {
        // Arrange
        var queen = _game.Board.GetPieceAt(new Position(3, 0)); // White Queen
        Assert.NotNull(queen);
        var pawnD2 = _game.Board.GetPieceAt(new Position(3, 1)); // White Pawn
        Assert.NotNull(pawnD2);
        
        _game.AddLoyaltyRelationship(new LoyaltyRelationship(pawnD2.Id, queen.Id));

        pawnD2.Loyalty = new LoyaltyValue(20); // Defecting (< 30)

        // Act
        _loyaltyManager.ProcessDefections();

        // Assert
        Assert.Equal(PlayerColor.Black, pawnD2.Color);
        Assert.Equal(50, pawnD2.Loyalty.Value);
        Assert.DoesNotContain(_game.LoyaltyRelationships, r => r.VassalId == pawnD2.Id || r.LordId == pawnD2.Id);
    }
}
