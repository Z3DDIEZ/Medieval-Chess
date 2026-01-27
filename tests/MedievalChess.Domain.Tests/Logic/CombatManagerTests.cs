using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities.Pieces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;
using Xunit;

namespace MedievalChess.Domain.Tests.Logic;

public class CombatManagerTests
{
    private readonly Game _game;
    private readonly CombatManager _combatManager;

    public CombatManagerTests()
    {
        _game = Game.StartNew();
        _combatManager = new CombatManager(_game);
    }

    [Fact]
    public void CalculateCombat_IsDeterministic_ForSameState()
    {
        // Arrange
        var attacker = new Knight(PlayerColor.White, new Position(0, 0));
        var defender = new Pawn(PlayerColor.Black, new Position(1, 1));
        
        // Act
        var result1 = _combatManager.CalculateCombat(attacker, defender);
        var result2 = _combatManager.CalculateCombat(attacker, defender);

        // Assert
        Assert.Equal(result1.DamageDealt, result2.DamageDealt);
        Assert.Equal(result1.IsCritical, result2.IsCritical);
    }

    [Fact]
    public void CalculateCombat_DamageScalesWithLevel()
    {
        // Arrange
        var attackerLevel1 = new Rook(PlayerColor.White, new Position(0, 0));
        var attackerLevel10 = new Rook(PlayerColor.White, new Position(0, 1));
        // Mock leveling up (since we can't set Level directly easily, assume default is 1)
        // We'll gain XP to level up attackerLevel10 9 times
        for(int i=1; i<10; i++)
        {
            attackerLevel10.GainXP(i * 100); 
        }

        var defender = new Pawn(PlayerColor.Black, new Position(1, 1));

        // Act & Assert
        // Due to Critical Hits (1.5x) and high variance, a lucky Level 1 can out-damage an unlucky Level 10.
        // We run multiple iterations to verify the trend (Average Damage should definitely be higher).

        double totalDmg1 = 0;
        double totalDmg10 = 0;
        int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Re-instantiate to get new IDs -> new Seeds -> new RNG
            var a1 = new Rook(PlayerColor.White, new Position(0, 0));
            var a10 = new Rook(PlayerColor.White, new Position(0, 1));
             for(int j=1; j<10; j++) a10.GainXP(j * 100);

            var d = new Pawn(PlayerColor.Black, new Position(1, 1));

            totalDmg1 += _combatManager.CalculateCombat(a1, d).DamageDealt;
            totalDmg10 += _combatManager.CalculateCombat(a10, d).DamageDealt;
        }

        double avg1 = totalDmg1 / iterations;
        double avg10 = totalDmg10 / iterations;

        Assert.True(avg10 > avg1, 
            $"Level 10 avg damage ({avg10}) should be higher than Level 1 ({avg1}) over {iterations} runs.");
    }

    [Fact]
    public void CalculateCombat_ArmorReducesDamage()
    {
        // Arrange
        var attacker = new Queen(PlayerColor.White, new Position(0, 0)); // Base 18
        var softDefender = new Pawn(PlayerColor.Black, new Position(1, 0)); // Armor 2
        var hardDefender = new King(PlayerColor.Black, new Position(7, 7)); // Armor 15

        // Act
        var resultSoft = _combatManager.CalculateCombat(attacker, softDefender);
        var resultHard = _combatManager.CalculateCombat(attacker, hardDefender);

        // Assert
        // We can't guarantee hard < soft due to RNG, but average is lower.
        // Hard armor reduces 7.5-15 damage. Soft reduces 1-2.
        // Diff ~10 damage. RNG on 18 base is +/- 20% (~3.6).
        // So Hard should definitely be lower.
        
        Assert.True(resultSoft.DamageDealt > resultHard.DamageDealt);
    }

    [Fact]
    public void GetPieceValue_ReturnsCorrectValues()
    {
        Assert.Equal(1, CombatManager.GetPieceValue(PieceType.Pawn));
        Assert.Equal(3, CombatManager.GetPieceValue(PieceType.Knight));
        Assert.Equal(3, CombatManager.GetPieceValue(PieceType.Bishop));
        Assert.Equal(5, CombatManager.GetPieceValue(PieceType.Rook));
        Assert.Equal(9, CombatManager.GetPieceValue(PieceType.Queen));
        Assert.Equal(4, CombatManager.GetPieceValue(PieceType.King));
    }
}
