using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RetenueSourceClientTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var retenue = new RetenueSourceClient
        {
            NumFacture = 10,
            NumTej = "TEJ-3",
            MontantAvantRetenu = 2000m,
            TauxRetenu = 1.5,
            MontantApresRetenu = 1970m,
            PdfStoragePath = "/docs/tej-client.pdf",
            DateCreation = new DateTime(2024, 7, 1),
            AccountingYearId = 2024
        };

        retenue.NumFacture.Should().Be(10);
        retenue.NumTej.Should().Be("TEJ-3");
        retenue.MontantAvantRetenu.Should().Be(2000m);
        retenue.TauxRetenu.Should().Be(1.5);
        retenue.MontantApresRetenu.Should().Be(1970m);
        retenue.PdfStoragePath.Should().Be("/docs/tej-client.pdf");
        retenue.DateCreation.Should().Be(new DateTime(2024, 7, 1));
        retenue.AccountingYearId.Should().Be(2024);
        retenue.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
