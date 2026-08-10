using TunNetCom.SharedKernel.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.SharedKernel;

public class DecimalHelperTest
{
    [Fact]
    public void RoundAmount_WhenMoreThanThreeDecimals_RoundsAwayFromZero()
    {
        var result = DecimalHelper.RoundAmount(10.5555m);

        result.Should().Be(10.556m);
    }

    [Fact]
    public void RoundAmount_WhenMidpoint_RoundsAwayFromZero()
    {
        var result = DecimalHelper.RoundAmount(10.1235m);

        result.Should().Be(10.124m);
    }

    [Fact]
    public void RoundAmount_WhenAlreadyThreeDecimals_ReturnsSameValue()
    {
        var result = DecimalHelper.RoundAmount(10.556m);

        result.Should().Be(10.556m);
    }

    [Fact]
    public void RoundAmount_WhenNegativeMidpoint_RoundsAwayFromZero()
    {
        var result = DecimalHelper.RoundAmount(-10.1235m);

        result.Should().Be(-10.124m);
    }

    [Fact]
    public void RoundPercentage_WhenMoreThanTwoDecimals_RoundsAwayFromZero()
    {
        var result = DecimalHelper.RoundPercentage(19.995m);

        result.Should().Be(20.00m);
    }

    [Fact]
    public void RoundPercentage_WhenMidpoint_RoundsAwayFromZero()
    {
        var result = DecimalHelper.RoundPercentage(0.005m);

        result.Should().Be(0.01m);
    }

    [Fact]
    public void RoundPercentage_WhenAlreadyTwoDecimals_ReturnsSameValue()
    {
        var result = DecimalHelper.RoundPercentage(19.99m);

        result.Should().Be(19.99m);
    }
}
