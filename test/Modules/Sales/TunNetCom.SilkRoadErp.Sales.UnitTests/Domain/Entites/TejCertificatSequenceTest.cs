using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class TejCertificatSequenceTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var sequence = new TejCertificatSequence
        {
            Annee = 2024,
            Mois = 3,
            DerniereSequence = 2,
            RowVersion = new byte[] { 1 }
        };

        sequence.Annee.Should().Be(2024);
        sequence.Mois.Should().Be(3);
        sequence.DerniereSequence.Should().Be(2);
        sequence.RowVersion.Should().BeEquivalentTo(new byte[] { 1 });
        sequence.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
