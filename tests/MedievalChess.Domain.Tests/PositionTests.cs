using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Tests;

public class PositionTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenCoordinatesAreInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Position(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Position(0, 8));
    }

    [Fact]
    public void FromAlgebraic_ShouldParseCorrectly()
    {
        var pos = Position.FromAlgebraic("e4");
        Assert.Equal(4, pos.File); // e is 5th letter, index 4
        Assert.Equal(3, pos.Rank); // 4 is 4th rank, index 3
    }

    [Fact]
    public void ToAlgebraic_ShouldFormatCorrectly()
    {
        var pos = new Position(0, 0);
        Assert.Equal("a1", pos.ToAlgebraic());
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameCoordinates()
    {
        var p1 = new Position(3, 3);
        var p2 = new Position(3, 3);
        Assert.Equal(p1, p2);
    }
}
