using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class FactureAvoirFournisseurTest
{
    [Fact]
    public void CreateFactureAvoirFournisseur_ShouldSetProperties()
    {
        var date = new DateTime(2024, 7, 1);

        var facture = FactureAvoirFournisseur.CreateFactureAvoirFournisseur(
            numFactureAvoirFourSurPage: 5,
            idFournisseur: 12,
            date: date,
            factureFournisseurId: 8,
            accountingYearId: 2024);

        facture.NumFactureAvoirFourSurPage.Should().Be(5);
        facture.IdFournisseur.Should().Be(12);
        facture.Date.Should().Be(date);
        facture.FactureFournisseurId.Should().Be(8);
        facture.AccountingYearId.Should().Be(2024);
        facture.Statut.Should().Be(DocumentStatus.Draft);
        facture.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void Valider_WhenDraft_ShouldSetValid()
    {
        var facture = FactureAvoirFournisseur.CreateFactureAvoirFournisseur(
            numFactureAvoirFourSurPage: 1,
            idFournisseur: 1,
            date: DateTime.Now,
            factureFournisseurId: null,
            accountingYearId: 2024);

        facture.Valider();

        facture.Statut.Should().Be(DocumentStatus.Valid);
    }

    [Fact]
    public void Valider_WhenAlreadyValid_ShouldThrow()
    {
        var facture = FactureAvoirFournisseur.CreateFactureAvoirFournisseur(
            numFactureAvoirFourSurPage: 1,
            idFournisseur: 1,
            date: DateTime.Now,
            factureFournisseurId: null,
            accountingYearId: 2024);
        facture.Valider();

        var act = () => facture.Valider();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un document en brouillon peut être validé.");
    }

    [Fact]
    public void UpdateFactureAvoirFournisseur_ShouldUpdateProperties()
    {
        var facture = FactureAvoirFournisseur.CreateFactureAvoirFournisseur(
            numFactureAvoirFourSurPage: 1,
            idFournisseur: 1,
            date: new DateTime(2024, 7, 1),
            factureFournisseurId: null,
            accountingYearId: 2024);
        var newDate = new DateTime(2024, 8, 1);

        facture.UpdateFactureAvoirFournisseur(
            numFactureAvoirFourSurPage: 9,
            idFournisseur: 3,
            date: newDate,
            factureFournisseurId: 22,
            accountingYearId: 2025);

        facture.NumFactureAvoirFourSurPage.Should().Be(9);
        facture.IdFournisseur.Should().Be(3);
        facture.Date.Should().Be(newDate);
        facture.FactureFournisseurId.Should().Be(22);
        facture.AccountingYearId.Should().Be(2025);
    }
}
