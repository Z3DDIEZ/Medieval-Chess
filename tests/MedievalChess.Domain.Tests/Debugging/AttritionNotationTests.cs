using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;
using MedievalChess.Domain.Logic; // For EngineService
using Xunit;

namespace MedievalChess.Domain.Tests.Debugging;

public class AttritionNotationTests
{
    private class MockEngine : IEngineService
    {
        public bool IsMoveLegal(Board board, Position from, Position to, PlayerColor turn, bool isAttritionMode = false) => true;
        public bool IsKingInCheck(Board board, PlayerColor color) => false;
        public bool IsCheckmate(Board board, PlayerColor color, bool isAttritionMode = false) => false;
        public bool IsStalemate(Board board, PlayerColor color, bool isAttritionMode = false) => false;
        public IEnumerable<Position> GetLegalDestinations(Board board, Position from, bool isAttritionMode = false) => new List<Position>();
    }

    private class MockRNG : Common.IRNGService
    {
        public int Next(int maxValue, int seed) => 0;
        public int Next(int minValue, int maxValue, int seed) => minValue;
        public double NextDouble(int seed) => 0.0;
        public bool RollChance(double percentage, int seed) => false;
    }

    private class MockNarrative : Common.INarrativeEngineService
    {
        public NarrativeEntry GenerateCombatNarrative(int turn, Piece attacker, Piece defender, int damage, bool isCrit, bool isGlancing) 
            => new NarrativeEntry(turn, NarratorType.System, "Hit", 5);
            
        public NarrativeEntry GenerateAbilityNarrative(int turnNumber, Piece caster, string abilityName)
             => new NarrativeEntry(turnNumber, NarratorType.System, "Ability", 5);

        public NarrativeEntry GenerateDefectionNarrative(int turnNumber, Piece traitor, Piece newLord)
             => new NarrativeEntry(turnNumber, NarratorType.System, "Defection", 10);
    }

    [Fact]
    public void AttackBounce_Should_Have_Distinguishable_Notation()
    {
        // Arrange
        var game = Game.StartNew();
        
        // Advance to turn 3 to enable Attrition Mode
        // T1 White
        game.ExecuteMove(new Position(0, 1), new Position(0, 2), new MockEngine(), new MockRNG(), new MockNarrative()); // a2-a3
        // T1 Black
        game.ExecuteMove(new Position(0, 6), new Position(0, 5), new MockEngine(), new MockRNG(), new MockNarrative()); // a7-a6
        // T2 White
        game.ExecuteMove(new Position(1, 1), new Position(1, 2), new MockEngine(), new MockRNG(), new MockNarrative()); // b2-b3
        // T2 Black
        game.ExecuteMove(new Position(1, 6), new Position(1, 5), new MockEngine(), new MockRNG(), new MockNarrative()); // b7-b6
        
        // Turn 3: Attrition Mode Activates
        Assert.True(game.IsAttritionMode, "Attrition Mode should be active on Turn 3");

        // Set up a combat scenario: White Pawn at d4, Black Pawn at e5
        var whitePawn = game.Board.GetPieceAt(new Position(3, 1)); // d2
        Assert.NotNull(whitePawn);
        whitePawn.MoveTo(new Position(3, 3)); // Teleport to d4
        
        var blackPawn = game.Board.GetPieceAt(new Position(4, 6)); // e7
        Assert.NotNull(blackPawn);
        blackPawn.MoveTo(new Position(4, 4)); // Teleport to e5
        
        // Act: White d4 attacks e5
        game.ExecuteMove(new Position(3, 3), new Position(4, 4), new MockEngine(), new MockRNG(), new MockNarrative());
        
        var lastMove = game.PlayedMoves.Last();
        
        // Assert
        // In Attrition Mode, a pawn attack that doesn't kill (MaxHP is high) should bounce.
        Assert.True(lastMove.IsAttackBounce, "Move should be a bounce");
        
        // The notation is currently "dxe5" which is confusing because the pawn stays at d4
        Console.WriteLine($"Notation: {lastMove.Notation}");
        
        // We WANT it to indicate bounce, e.g. "dxe5 (Bounce)" or similar
        Assert.True(lastMove.Notation.Contains("Bounce") || lastMove.Notation.Contains("*"), "Notation should indicate bounce");
    }
}
