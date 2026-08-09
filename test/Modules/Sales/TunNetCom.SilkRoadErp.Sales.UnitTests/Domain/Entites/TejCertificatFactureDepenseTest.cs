using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class TejCertificatFactureDepenseTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var certificat = new TejCertificatFactureDepense
        {
            FactureDepenseId = 20,
            RefCertif = "3002"
        };

        certificat.FactureDepenseId.Should().Be(20);
        certificat.RefCertif.Should().Be("3002");
        certificat.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
