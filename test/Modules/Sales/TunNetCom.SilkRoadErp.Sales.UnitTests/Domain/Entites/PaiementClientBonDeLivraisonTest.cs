using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementClientBonDeLivraisonTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var link = PaiementClientBonDeLivraison.Create(paiementClientId: 1, bonDeLivraisonId: 2);

        link.PaiementClientId.Should().Be(1);
        link.BonDeLivraisonId.Should().Be(2);
        link.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
