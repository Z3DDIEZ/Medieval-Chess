using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Tests;

public class LoyaltyValueTests
{
    [Theory]
    [InlineData(95, LoyaltyState.Devoted)]
    [InlineData(75, LoyaltyState.Loyal)]
    [InlineData(55, LoyaltyState.Wavering)]
    [InlineData(35, LoyaltyState.Disloyal)]
    [InlineData(10, LoyaltyState.Defecting)]
    public void State_ShouldMapCorrectly(int value, LoyaltyState expectedState)
    {
        var loyalty = new LoyaltyValue(value);
        Assert.Equal(expectedState, loyalty.State);
    }

    [Fact]
    public void Constructor_ShouldClampValues()
    {
        var l1 = new LoyaltyValue(150);
        Assert.Equal(100, l1.Value);

        var l2 = new LoyaltyValue(-50);
        Assert.Equal(0, l2.Value);
    }

    [Fact]
    public void Adjust_ShouldCreateNewInstance()
    {
        var l1 = new LoyaltyValue(50);
        var l2 = l1.Adjust(-10);
        
        Assert.Equal(50, l1.Value); // Immutable
        Assert.Equal(40, l2.Value);
    }
}
