using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Services;

public class ReceiptNoteFodecCalculatorTest
{
    [Fact]
    public void CalculateFodecAndTtc_WhenNotConstructor_ShouldReturnZeroFodec()
    {
        var (fodec, totTtc) = ReceiptNoteFodecCalculator.CalculateFodecAndTtc(
            totHt: 1000m,
            vatRate: 19,
            fodecRate: 1,
            isConstructor: false);

        fodec.Should().Be(0);
        totTtc.Should().Be(1190m);
    }

    [Fact]
    public void CalculateFodecAndTtc_WhenTotHtZero_ShouldReturnZero()
    {
        var (fodec, totTtc) = ReceiptNoteFodecCalculator.CalculateFodecAndTtc(
            totHt: 0m,
            vatRate: 19,
            fodecRate: 1,
            isConstructor: true);

        fodec.Should().Be(0);
        totTtc.Should().Be(0m);
    }

    [Fact]
    public void CalculateFodecAndTtc_WhenConstructor_ShouldComputeFodecAndTtc()
    {
        var (fodec, totTtc) = ReceiptNoteFodecCalculator.CalculateFodecAndTtc(
            totHt: 1000m,
            vatRate: 19,
            fodecRate: 5,
            isConstructor: true);

        // FODEC = 5% of 1000 = 50
        // TVA base = 1000 + 50 = 1050, TVA = 19% of 1050 = 199.5
        // TTC = 1000 + 50 + 199.5 = 1249.5
        fodec.Should().Be(50m);
        totTtc.Should().Be(1249.5m);
    }

    [Fact]
    public void CalculateFodecAndTtc_WhenConstructorWithZeroFodecRate_ShouldComputeTtcOnHt()
    {
        var (fodec, totTtc) = ReceiptNoteFodecCalculator.CalculateFodecAndTtc(
            totHt: 100m,
            vatRate: 19,
            fodecRate: 0,
            isConstructor: true);

        fodec.Should().Be(0m);
        totTtc.Should().Be(119m);
    }
}
