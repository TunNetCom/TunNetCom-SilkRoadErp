using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class TejCertificatFactureTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var certificat = new TejCertificatFacture
        {
            FactureFournisseurId = 10,
            RefCertif = "3001"
        };

        certificat.FactureFournisseurId.Should().Be(10);
        certificat.RefCertif.Should().Be("3001");
        certificat.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
