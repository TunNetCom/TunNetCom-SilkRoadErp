using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RetenueSourceFournisseurTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var retenue = new RetenueSourceFournisseur
        {
            NumFactureFournisseur = 10,
            NumTej = "TEJ-1",
            MontantAvantRetenu = 1000m,
            TauxRetenu = 1.5,
            MontantApresRetenu = 985m,
            PdfStoragePath = "/docs/tej.pdf",
            DateCreation = new DateTime(2024, 7, 1),
            AccountingYearId = 2024
        };

        retenue.NumFactureFournisseur.Should().Be(10);
        retenue.NumTej.Should().Be("TEJ-1");
        retenue.MontantAvantRetenu.Should().Be(1000m);
        retenue.TauxRetenu.Should().Be(1.5);
        retenue.MontantApresRetenu.Should().Be(985m);
        retenue.PdfStoragePath.Should().Be("/docs/tej.pdf");
        retenue.DateCreation.Should().Be(new DateTime(2024, 7, 1));
        retenue.AccountingYearId.Should().Be(2024);
        retenue.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
