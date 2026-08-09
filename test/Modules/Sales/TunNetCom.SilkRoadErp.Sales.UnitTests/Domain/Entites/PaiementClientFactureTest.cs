using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementClientFactureTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var link = PaiementClientFacture.Create(paiementClientId: 1, factureId: 2);

        link.PaiementClientId.Should().Be(1);
        link.FactureId.Should().Be(2);
        link.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
