using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class FactureAvoirClientTest
{
    [Fact]
    public void CreateFactureAvoirClient_ShouldSetProperties()
    {
        var date = new DateTime(2024, 7, 1);

        var facture = FactureAvoirClient.CreateFactureAvoirClient(
            numFactureAvoirClientSurPage: 3,
            idClient: 2,
            date: date,
            numFacture: 100,
            accountingYearId: 2024);

        facture.NumFactureAvoirClientSurPage.Should().Be(3);
        facture.IdClient.Should().Be(2);
        facture.Date.Should().Be(date);
        facture.NumFacture.Should().Be(100);
        facture.AccountingYearId.Should().Be(2024);
        facture.Statut.Should().Be(DocumentStatus.Draft);
        facture.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void CreateFactureAvoirClient_WhenNumFactureNull_ShouldKeepNull()
    {
        var facture = FactureAvoirClient.CreateFactureAvoirClient(
            numFactureAvoirClientSurPage: 1,
            idClient: 1,
            date: DateTime.Now,
            numFacture: null,
            accountingYearId: 2024);

        facture.NumFacture.Should().BeNull();
    }

    [Fact]
    public void Valider_WhenDraft_ShouldSetValid()
    {
        var facture = FactureAvoirClient.CreateFactureAvoirClient(1, 1, DateTime.Now, null, 2024);

        facture.Valider();

        facture.Statut.Should().Be(DocumentStatus.Valid);
    }

    [Fact]
    public void Valider_WhenAlreadyValid_ShouldThrow()
    {
        var facture = FactureAvoirClient.CreateFactureAvoirClient(1, 1, DateTime.Now, null, 2024);
        facture.Valider();

        var act = () => facture.Valider();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Seul un document en brouillon peut être validé.");
    }

    [Fact]
    public void UpdateFactureAvoirClient_ShouldUpdateProperties()
    {
        var facture = FactureAvoirClient.CreateFactureAvoirClient(1, 1, DateTime.Now, null, 2024);

        facture.UpdateFactureAvoirClient(
            numFactureAvoirClientSurPage: 5,
            idClient: 3,
            date: new DateTime(2024, 8, 1),
            numFacture: 200,
            accountingYearId: 2025);

        facture.NumFactureAvoirClientSurPage.Should().Be(5);
        facture.IdClient.Should().Be(3);
        facture.Date.Should().Be(new DateTime(2024, 8, 1));
        facture.NumFacture.Should().Be(200);
        facture.AccountingYearId.Should().Be(2025);
    }
}
