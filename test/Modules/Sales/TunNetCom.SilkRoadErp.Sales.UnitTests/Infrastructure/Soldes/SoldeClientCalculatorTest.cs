using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.Soldes;

public class SoldeClientCalculatorTest
{
    [Fact]
    public void ComputeMontantFactureClient_WhenRetenueExists_ReturnsMontantApresRetenu()
    {
        var retenues = new Dictionary<int, decimal>
        {
            [42] = 900m
        };

        var result = SoldeClientCalculator.ComputeMontantFactureClient(42, retenues, 1000m, 1m);

        _ = result.Should().Be(900m);
    }

    [Fact]
    public void ComputeMontantFactureClient_WhenNoRetenue_ReturnsSumNetPayerPlusTimbre()
    {
        var result = SoldeClientCalculator.ComputeMontantFactureClient(7, new Dictionary<int, decimal>(), 1000m, 1m);

        _ = result.Should().Be(1001m);
    }

    [Fact]
    public void ComputeMontantFactureClient_WhenNoRetenueAndNoBl_ReturnsTimbre()
    {
        var result = SoldeClientCalculator.ComputeMontantFactureClient(7, new Dictionary<int, decimal>(), 0m, 5m);

        _ = result.Should().Be(5m);
    }

    [Fact]
    public void ComputeSolde_ReturnsAvoirsPlusFacturesAvoirPlusPaiementsMinusFacturesMinusBls()
    {
        var result = SoldeClientCalculator.ComputeSolde(
            totalFactures: 1000m,
            totalBonsLivraisonNonFactures: 200m,
            totalAvoirs: 300m,
            totalFacturesAvoir: 400m,
            totalPaiements: 100m);

        _ = result.Should().Be(300m + 400m + 100m - 1000m - 200m);
    }

    [Fact]
    public void ComputeSolde_WithZeroes_ReturnsZero()
    {
        var result = SoldeClientCalculator.ComputeSolde(0m, 0m, 0m, 0m, 0m);

        _ = result.Should().Be(0m);
    }
}
