using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RetenueSourceFactureDepenseTest
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var retenue = new RetenueSourceFactureDepense
        {
            FactureDepenseId = 10,
            NumTej = "TEJ-2",
            MontantAvantRetenu = 500m,
            TauxRetenu = 2.5,
            MontantApresRetenu = 487.5m,
            PdfStoragePath = "/docs/tej-depense.pdf",
            DateCreation = new DateTime(2024, 7, 1),
            AccountingYearId = 2024
        };

        retenue.FactureDepenseId.Should().Be(10);
        retenue.NumTej.Should().Be("TEJ-2");
        retenue.MontantAvantRetenu.Should().Be(500m);
        retenue.TauxRetenu.Should().Be(2.5);
        retenue.MontantApresRetenu.Should().Be(487.5m);
        retenue.PdfStoragePath.Should().Be("/docs/tej-depense.pdf");
        retenue.DateCreation.Should().Be(new DateTime(2024, 7, 1));
        retenue.AccountingYearId.Should().Be(2024);
        retenue.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
