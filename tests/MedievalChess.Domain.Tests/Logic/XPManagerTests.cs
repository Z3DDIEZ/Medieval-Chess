using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities.Pieces;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Logic;
using Xunit;

namespace MedievalChess.Domain.Tests.Logic;

public class XPManagerTests
{
    private readonly Game _game;
    private readonly XPManager _xpManager;

    public XPManagerTests()
    {
        _game = Game.StartNew();
        _xpManager = new XPManager(_game);
    }

    [Fact]
    public void AwardXP_IncreasesTotalXPAndLevelsUp()
    {
        // Arrange
        var knight = new Knight(PlayerColor.White, new Primitives.Position(0, 0));
        Assert.Equal(1, knight.Level);
        Assert.Equal(0, knight.XP);
        Assert.Equal(0, knight.TotalXP);

        // Act
        // Level 1 to 2 requires 100 XP
        _xpManager.AwardAbilityXP(knight); // Gives 3 XP
        
        // Assert
        Assert.Equal(3, knight.XP);
        Assert.Equal(3, knight.TotalXP);
        Assert.Equal(1, knight.Level);

        // Act
        knight.GainXP(100); // 103 XP total
        // We must artificially trigger CheckLevelUp through AwardSurvivalXP or generic AwardXP
        // GainXP directly doesn't trigger level up unless going through manager
        _xpManager.AwardSurvivalXP(knight); // Gives 5 XP (108 total)

        // Assert
        Assert.Equal(108, knight.XP);
        Assert.Equal(108, knight.TotalXP);
        Assert.Equal(2, knight.Level); // Leveled up!
    }

    [Fact]
    public void SpendingXP_DoesNotAffectLevelingBoundary()
    {
        // Arrange
        var knight = new Knight(PlayerColor.White, new Primitives.Position(0, 0));
        
        // Let's get to 90 XP (close to Level 2)
        knight.GainXP(90);

        // We spend 30 XP on an ability
        knight.SpendXP(30);

        // Assert currency is 60, but total is 90
        Assert.Equal(60, knight.XP);
        Assert.Equal(90, knight.TotalXP);
        Assert.Equal(1, knight.Level);

        // Act - we earn 15 more XP, which puts TotalXP at 105 (>100)
        // Currency will become 60 + 15 = 75
        _xpManager.AwardCheckXP(knight); // Gives 15 XP

        // Assert - piece should level up despite currency being only 75
        Assert.Equal(75, knight.XP);
        Assert.Equal(105, knight.TotalXP);
        Assert.Equal(2, knight.Level);
    }
}
