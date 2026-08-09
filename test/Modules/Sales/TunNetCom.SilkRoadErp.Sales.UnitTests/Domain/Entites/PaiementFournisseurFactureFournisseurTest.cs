using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementFournisseurFactureFournisseurTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var link = PaiementFournisseurFactureFournisseur.Create(paiementFournisseurId: 1, factureFournisseurId: 2);

        link.PaiementFournisseurId.Should().Be(1);
        link.FactureFournisseurId.Should().Be(2);
        link.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
