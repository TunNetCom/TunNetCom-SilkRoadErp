using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementTiersDepenseFactureDepenseTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var link = PaiementTiersDepenseFactureDepense.Create(paiementTiersDepenseId: 1, factureDepenseId: 2);

        link.PaiementTiersDepenseId.Should().Be(1);
        link.FactureDepenseId.Should().Be(2);
        link.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
