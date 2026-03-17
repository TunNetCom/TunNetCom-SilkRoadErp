namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Tests.DecimalHelper;

public class DecimalHelperTests
{
    private static decimal RoundAmount(decimal value) => TunNetCom.SilkRoadErp.Sales.Domain.Services.DecimalHelper.RoundAmount(value);
    private static decimal RoundPercentage(decimal value) => TunNetCom.SilkRoadErp.Sales.Domain.Services.DecimalHelper.RoundPercentage(value);

    #region RoundAmount

    [Fact]
    public void RoundAmount_Zero_ReturnsZero()
    {
        var result = RoundAmount(0m);
        Assert.Equal(0m, result);
    }

    [Theory]
    [InlineData(1.2344, 1.234)]
    [InlineData(1.2345, 1.235)]
    [InlineData(1.2346, 1.235)]
    [InlineData(10.9994, 10.999)]
    [InlineData(10.9995, 11.000)]
    [InlineData(10.9996, 11.000)]
    public void RoundAmount_RoundsToThreeDecimalPlaces_AwayFromZero(decimal value, decimal expected)
    {
        var result = RoundAmount(value);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundAmount_NegativeValue_RoundsAwayFromZero()
    {
        Assert.Equal(-1.234m, RoundAmount(-1.2335m));
        Assert.Equal(-1.235m, RoundAmount(-1.2345m));
    }

    [Theory]
    [InlineData(1.2, 1.2)]
    [InlineData(1.23, 1.23)]
    [InlineData(1.234, 1.234)]
    [InlineData(100.1, 100.1)]
    public void RoundAmount_FewerThanThreeDecimals_Unchanged(decimal value, decimal expected)
    {
        var result = RoundAmount(value);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundAmount_MoreThanThreeDecimals_TruncatesToThree()
    {
        var result = RoundAmount(1.2345678m);
        Assert.Equal(1.235m, result);
    }

    #endregion

    #region RoundPercentage

    [Fact]
    public void RoundPercentage_Zero_ReturnsZero()
    {
        var result = RoundPercentage(0m);
        Assert.Equal(0m, result);
    }

    [Theory]
    [InlineData(19.994, 19.99)]
    [InlineData(19.995, 20.00)]
    [InlineData(19.996, 20.00)]
    [InlineData(7.124, 7.12)]
    [InlineData(7.125, 7.13)]
    [InlineData(7.126, 7.13)]
    public void RoundPercentage_RoundsToTwoDecimalPlaces_AwayFromZero(decimal value, decimal expected)
    {
        var result = RoundPercentage(value);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundPercentage_NegativeValue_RoundsAwayFromZero()
    {
        Assert.Equal(-19.99m, RoundPercentage(-19.985m));
        Assert.Equal(-20.00m, RoundPercentage(-19.995m));
    }

    [Theory]
    [InlineData(19.9, 19.9)]
    [InlineData(19.99, 19.99)]
    [InlineData(7.0, 7.0)]
    public void RoundPercentage_FewerThanTwoDecimals_Unchanged(decimal value, decimal expected)
    {
        var result = RoundPercentage(value);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundPercentage_MoreThanTwoDecimals_TruncatesToTwo()
    {
        var result = RoundPercentage(13.456m);
        Assert.Equal(13.46m, result);
    }

    #endregion
}
