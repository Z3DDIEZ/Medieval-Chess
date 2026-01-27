using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Entities.Pieces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Logic;
using MedievalChess.Domain.Primitives;
using Xunit;

namespace MedievalChess.Domain.Tests.Logic;

public class AbilityManagerTests
{
    private readonly Game _game;
    private readonly AbilityManager _abilityManager;

    public AbilityManagerTests()
    {
        _game = Game.StartNew();
        _abilityManager = new AbilityManager(_game);
    }

    [Fact]
    public void AdvanceCooldowns_ReducesCooldownByOne()
    {
        // Arrange
        var pawn = _game.Board.GetPieceAt(new Position(0, 1));
        var ability = new PieceAbility(pawn.Id, Guid.NewGuid(), 3);
        ability.TriggerCooldown(); // Set to 3
        pawn.Abilities.Add(ability);

        // Act
        _abilityManager.AdvanceCooldowns();

        // Assert
        Assert.Equal(2, ability.CurrentCooldown);
    }

    [Fact]
    public void TickEffects_ReducesDurationAndRemovesExpired()
    {
        // Arrange
        var pawn = _game.Board.GetPieceAt(new Position(0, 1));
        var effect = new ActiveEffect(pawn.Id, EffectType.DamageReduction, 10, 1);
        pawn.ActiveEffects.Add(effect);

        // Act 1
        _abilityManager.TickEffects();

        // Assert 1: Should now be 0 duration, still in list
        Assert.Equal(0, effect.RemainingDuration);
        Assert.Contains(effect, pawn.ActiveEffects);

        // Act 2
        _abilityManager.TickEffects();

        // Assert 2: Should be removed
        Assert.DoesNotContain(effect, pawn.ActiveEffects);
    }
}
