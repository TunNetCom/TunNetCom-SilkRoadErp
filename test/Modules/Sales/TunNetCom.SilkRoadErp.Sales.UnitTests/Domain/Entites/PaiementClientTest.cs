using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementClientTest
{
    private static PaiementClient CreatePaiement()
    {
        return PaiementClient.CreatePaiementClient(
            numeroTransactionBancaire: "TRX-001",
            clientId: 1,
            accountingYearId: 2024,
            montant: 500m,
            datePaiement: new DateTime(2024, 7, 15),
            methodePaiement: MethodePaiement.Cheque,
            factureIds: new[] { 1, 2 },
            bonDeLivraisonIds: new[] { 10, 11 },
            numeroChequeTraite: "CHQ-100",
            banqueId: 3,
            dateEcheance: new DateTime(2024, 8, 15),
            commentaire: "Paiement client",
            documentStoragePath: "/docs/paiement.pdf");
    }

    [Fact]
    public void CreatePaiementClient_ShouldSetProperties()
    {
        var paiement = CreatePaiement();

        paiement.NumeroTransactionBancaire.Should().Be("TRX-001");
        paiement.ClientId.Should().Be(1);
        paiement.AccountingYearId.Should().Be(2024);
        paiement.Montant.Should().Be(500m);
        paiement.DatePaiement.Should().Be(new DateTime(2024, 7, 15));
        paiement.MethodePaiement.Should().Be(MethodePaiement.Cheque);
        paiement.NumeroChequeTraite.Should().Be("CHQ-100");
        paiement.BanqueId.Should().Be(3);
        paiement.DateEcheance.Should().Be(new DateTime(2024, 8, 15));
        paiement.Commentaire.Should().Be("Paiement client");
        paiement.DocumentStoragePath.Should().Be("/docs/paiement.pdf");
        paiement.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        paiement.Factures.Should().BeEmpty();
        paiement.BonDeLivraisons.Should().BeEmpty();
    }

    [Fact]
    public void CreatePaiementClient_WithNullOptionalValues_ShouldKeepNull()
    {
        var paiement = PaiementClient.CreatePaiementClient(
            numeroTransactionBancaire: null,
            clientId: 1,
            accountingYearId: 2024,
            montant: 100m,
            datePaiement: DateTime.Now,
            methodePaiement: MethodePaiement.Espece,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            documentStoragePath: null);

        paiement.NumeroTransactionBancaire.Should().BeNull();
        paiement.NumeroChequeTraite.Should().BeNull();
        paiement.BanqueId.Should().BeNull();
        paiement.DateEcheance.Should().BeNull();
        paiement.Commentaire.Should().BeNull();
        paiement.DocumentStoragePath.Should().BeNull();
    }

    [Fact]
    public void UpdatePaiementClient_ShouldUpdateProperties()
    {
        var paiement = CreatePaiement();

        paiement.UpdatePaiementClient(
            numeroTransactionBancaire: "TRX-002",
            clientId: 2,
            accountingYearId: 2025,
            montant: 700m,
            datePaiement: new DateTime(2025, 1, 10),
            methodePaiement: MethodePaiement.Virement,
            factureIds: new[] { 5 },
            bonDeLivraisonIds: new[] { 20 },
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: "Updated",
            documentStoragePath: "/docs/updated.pdf");

        paiement.NumeroTransactionBancaire.Should().Be("TRX-002");
        paiement.ClientId.Should().Be(2);
        paiement.AccountingYearId.Should().Be(2025);
        paiement.Montant.Should().Be(700m);
        paiement.MethodePaiement.Should().Be(MethodePaiement.Virement);
        paiement.Commentaire.Should().Be("Updated");
        paiement.DateModification.Should().NotBeNull();
        paiement.Factures.Should().HaveCount(1);
        paiement.Factures.Should().ContainSingle().Which.FactureId.Should().Be(5);
        paiement.BonDeLivraisons.Should().HaveCount(1);
        paiement.BonDeLivraisons.Should().ContainSingle().Which.BonDeLivraisonId.Should().Be(20);
    }

    [Fact]
    public void UpdatePaiementClient_WhenIdsNull_ShouldKeepExistingCollections()
    {
        var paiement = CreatePaiement();

        paiement.UpdatePaiementClient(
            numeroTransactionBancaire: "TRX-002",
            clientId: 1,
            accountingYearId: 2024,
            montant: 500m,
            datePaiement: new DateTime(2024, 7, 15),
            methodePaiement: MethodePaiement.Cheque,
            factureIds: null,
            bonDeLivraisonIds: null,
            numeroChequeTraite: "CHQ-100",
            banqueId: 3,
            dateEcheance: new DateTime(2024, 8, 15),
            commentaire: "Paiement client",
            documentStoragePath: "/docs/paiement.pdf");

        paiement.Factures.Should().BeEmpty();
        paiement.BonDeLivraisons.Should().BeEmpty();
    }
}
