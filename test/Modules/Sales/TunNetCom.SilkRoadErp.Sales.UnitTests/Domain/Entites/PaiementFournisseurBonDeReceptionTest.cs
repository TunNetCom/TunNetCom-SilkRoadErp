using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementFournisseurBonDeReceptionTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var link = PaiementFournisseurBonDeReception.Create(paiementFournisseurId: 1, bonDeReceptionId: 2);

        link.PaiementFournisseurId.Should().Be(1);
        link.BonDeReceptionId.Should().Be(2);
        link.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
