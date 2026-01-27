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

        // Act
        // Use same seeded RNG context if possible, but IDs differ.
        // We test general magnitude.
        // Base Rook Damage = 10.
        // Level 1 (x1.1) = ~11.
        // Level 10 (x2.0) = ~20.
        
        var result1 = _combatManager.CalculateCombat(attackerLevel1, defender);
        var result10 = _combatManager.CalculateCombat(attackerLevel10, defender);

        // Assert
        // Given variances (0.8-1.2) and Armor (2 * 0.5-1.0 = 1-2 reduction)
        // Level 1: 10 * 1.1 * 0.8 - 2 = ~6.8. Max: 10 * 1.1 * 1.2 - 1 = ~12.2.
        // Level 10: 10 * 2.0 * 0.8 - 2 = ~14. Max: 10 * 2.0 * 1.2 - 1 = ~23.
        
        Assert.True(result10.DamageDealt > result1.DamageDealt, 
            $"Level 10 damage ({result10.DamageDealt}) should generally be higher than Level 1 ({result1.DamageDealt})");
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
