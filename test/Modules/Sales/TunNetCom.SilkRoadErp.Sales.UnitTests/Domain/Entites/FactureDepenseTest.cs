using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class FactureDepenseTest
{
    [Fact]
    public void Create_ShouldSetDefaults()
    {
        var date = new DateTime(2024, 7, 1);

        var facture = FactureDepense.Create(
            num: 3,
            idTiersDepenseFonctionnement: 2,
            date: date,
            description: "Loyer",
            montantTotal: 1000m,
            accountingYearId: 2024);

        facture.Num.Should().Be(3);
        facture.IdTiersDepenseFonctionnement.Should().Be(2);
        facture.Date.Should().Be(date);
        facture.Description.Should().Be("Loyer");
        facture.MontantTotal.Should().Be(1000m);
        facture.AccountingYearId.Should().Be(2024);
        facture.Statut.Should().Be(DocumentStatus.Draft);
        facture.BaseHT0.Should().Be(0m);
        facture.MontantTVA0.Should().Be(0m);
        facture.BaseHT7.Should().Be(0m);
        facture.MontantTVA7.Should().Be(0m);
        facture.BaseHT13.Should().Be(0m);
        facture.MontantTVA13.Should().Be(0m);
        facture.BaseHT19.Should().Be(0m);
        facture.MontantTVA19.Should().Be(0m);
        facture.DocumentStoragePath.Should().BeNull();
        facture.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void Create_WithTvaValues_ShouldSetThem()
    {
        var facture = FactureDepense.Create(
            num: 1,
            idTiersDepenseFonctionnement: 1,
            date: DateTime.Now,
            description: null!,
            montantTotal: 1190m,
            accountingYearId: 2024,
            documentStoragePath: "/docs/facture.pdf",
            baseHT0: 100m,
            montantTVA0: 0m,
            baseHT7: 200m,
            montantTVA7: 14m,
            baseHT13: 300m,
            montantTVA13: 39m,
            baseHT19: 400m,
            montantTVA19: 76m);

        facture.Description.Should().BeEmpty();
        facture.DocumentStoragePath.Should().Be("/docs/facture.pdf");
        facture.BaseHT0.Should().Be(100m);
        facture.BaseHT7.Should().Be(200m);
        facture.BaseHT13.Should().Be(300m);
        facture.BaseHT19.Should().Be(400m);
        facture.MontantTVA19.Should().Be(76m);
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        var facture = FactureDepense.Create(
            num: 1,
            idTiersDepenseFonctionnement: 1,
            date: new DateTime(2024, 7, 1),
            description: "Old",
            montantTotal: 100m,
            accountingYearId: 2024);
        var newDate = new DateTime(2024, 8, 1);

        facture.Update(
            date: newDate,
            description: "New",
            montantTotal: 200m,
            documentStoragePath: "/docs/new.pdf",
            baseHT19: 168m,
            montantTVA19: 32m);

        facture.Date.Should().Be(newDate);
        facture.Description.Should().Be("New");
        facture.MontantTotal.Should().Be(200m);
        facture.DocumentStoragePath.Should().Be("/docs/new.pdf");
        facture.BaseHT19.Should().Be(168m);
        facture.MontantTVA19.Should().Be(32m);
    }

    [Fact]
    public void Valider_WhenDraft_ShouldSetValid()
    {
        var facture = FactureDepense.Create(
            num: 1,
            idTiersDepenseFonctionnement: 1,
            date: DateTime.Now,
            description: "Test",
            montantTotal: 100m,
            accountingYearId: 2024);

        facture.Valider();

        facture.Statut.Should().Be(DocumentStatus.Valid);
    }

    [Fact]
    public void Valider_WhenAlreadyValid_ShouldThrow()
    {
        var facture = FactureDepense.Create(
            num: 1,
            idTiersDepenseFonctionnement: 1,
            date: DateTime.Now,
            description: "Test",
            montantTotal: 100m,
            accountingYearId: 2024);
        facture.Valider();

        var act = () => facture.Valider();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un document en brouillon peut être validé.");
    }
}
