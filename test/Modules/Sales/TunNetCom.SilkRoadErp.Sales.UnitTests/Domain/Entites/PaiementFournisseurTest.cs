using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementFournisseurTest
{
    private static PaiementFournisseur CreatePaiement()
    {
        return PaiementFournisseur.CreatePaiementFournisseur(
            numeroTransactionBancaire: "TRX-001",
            fournisseurId: 1,
            accountingYearId: 2024,
            montant: 500m,
            datePaiement: new DateTime(2024, 7, 15),
            methodePaiement: MethodePaiement.Cheque,
            factureFournisseurIds: new[] { 1, 2 },
            bonDeReceptionIds: new[] { 10, 11 },
            numeroChequeTraite: "CHQ-100",
            banqueId: 3,
            dateEcheance: new DateTime(2024, 8, 15),
            commentaire: "Paiement fournisseur",
            ribCodeEtab: "011",
            ribCodeAgence: "123",
            ribNumeroCompte: "456789",
            ribCle: "25",
            documentStoragePath: "/docs/paiement.pdf",
            mois: 7);
    }

    [Fact]
    public void CreatePaiementFournisseur_ShouldSetProperties()
    {
        var paiement = CreatePaiement();

        paiement.NumeroTransactionBancaire.Should().Be("TRX-001");
        paiement.FournisseurId.Should().Be(1);
        paiement.AccountingYearId.Should().Be(2024);
        paiement.Montant.Should().Be(500m);
        paiement.DatePaiement.Should().Be(new DateTime(2024, 7, 15));
        paiement.MethodePaiement.Should().Be(MethodePaiement.Cheque);
        paiement.NumeroChequeTraite.Should().Be("CHQ-100");
        paiement.BanqueId.Should().Be(3);
        paiement.DateEcheance.Should().Be(new DateTime(2024, 8, 15));
        paiement.Commentaire.Should().Be("Paiement fournisseur");
        paiement.RibCodeEtab.Should().Be("011");
        paiement.RibCodeAgence.Should().Be("123");
        paiement.RibNumeroCompte.Should().Be("456789");
        paiement.RibCle.Should().Be("25");
        paiement.DocumentStoragePath.Should().Be("/docs/paiement.pdf");
        paiement.Mois.Should().Be(7);
        paiement.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        paiement.FactureFournisseurs.Should().BeEmpty();
        paiement.BonDeReceptions.Should().BeEmpty();
    }

    [Fact]
    public void CreatePaiementFournisseur_WithNullOptionalValues_ShouldKeepNull()
    {
        var paiement = PaiementFournisseur.CreatePaiementFournisseur(
            numeroTransactionBancaire: null,
            fournisseurId: 1,
            accountingYearId: 2024,
            montant: 100m,
            datePaiement: DateTime.Now,
            methodePaiement: MethodePaiement.Espece,
            factureFournisseurIds: null,
            bonDeReceptionIds: null,
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: null,
            ribCodeEtab: null,
            ribCodeAgence: null,
            ribNumeroCompte: null,
            ribCle: null,
            documentStoragePath: null,
            mois: null);

        paiement.NumeroTransactionBancaire.Should().BeNull();
        paiement.NumeroChequeTraite.Should().BeNull();
        paiement.BanqueId.Should().BeNull();
        paiement.DateEcheance.Should().BeNull();
        paiement.Commentaire.Should().BeNull();
        paiement.DocumentStoragePath.Should().BeNull();
        paiement.Mois.Should().BeNull();
    }

    [Fact]
    public void UpdatePaiementFournisseur_ShouldUpdateProperties()
    {
        var paiement = CreatePaiement();

        paiement.UpdatePaiementFournisseur(
            numeroTransactionBancaire: "TRX-002",
            fournisseurId: 2,
            accountingYearId: 2025,
            montant: 700m,
            datePaiement: new DateTime(2025, 1, 10),
            methodePaiement: MethodePaiement.Virement,
            factureFournisseurIds: new[] { 5 },
            bonDeReceptionIds: new[] { 20 },
            numeroChequeTraite: null,
            banqueId: null,
            dateEcheance: null,
            commentaire: "Updated",
            ribCodeEtab: "022",
            ribCodeAgence: "456",
            ribNumeroCompte: "123456",
            ribCle: "74",
            documentStoragePath: "/docs/updated.pdf",
            mois: 1);

        paiement.NumeroTransactionBancaire.Should().Be("TRX-002");
        paiement.FournisseurId.Should().Be(2);
        paiement.AccountingYearId.Should().Be(2025);
        paiement.Montant.Should().Be(700m);
        paiement.DatePaiement.Should().Be(new DateTime(2025, 1, 10));
        paiement.MethodePaiement.Should().Be(MethodePaiement.Virement);
        paiement.Commentaire.Should().Be("Updated");
        paiement.RibCodeEtab.Should().Be("022");
        paiement.Mois.Should().Be(1);
        paiement.DateModification.Should().NotBeNull();
        paiement.FactureFournisseurs.Should().HaveCount(1);
        paiement.FactureFournisseurs.Should().ContainSingle().Which.FactureFournisseurId.Should().Be(5);
        paiement.BonDeReceptions.Should().HaveCount(1);
        paiement.BonDeReceptions.Should().ContainSingle().Which.BonDeReceptionId.Should().Be(20);
    }

    [Fact]
    public void UpdatePaiementFournisseur_WhenIdsNull_ShouldKeepExistingCollections()
    {
        var paiement = CreatePaiement();

        paiement.UpdatePaiementFournisseur(
            numeroTransactionBancaire: "TRX-002",
            fournisseurId: 1,
            accountingYearId: 2024,
            montant: 500m,
            datePaiement: new DateTime(2024, 7, 15),
            methodePaiement: MethodePaiement.Cheque,
            factureFournisseurIds: null,
            bonDeReceptionIds: null,
            numeroChequeTraite: "CHQ-100",
            banqueId: 3,
            dateEcheance: new DateTime(2024, 8, 15),
            commentaire: "Paiement fournisseur",
            ribCodeEtab: "011",
            ribCodeAgence: "123",
            ribNumeroCompte: "456789",
            ribCle: "25",
            documentStoragePath: "/docs/paiement.pdf",
            mois: 7);

        paiement.FactureFournisseurs.Should().BeEmpty();
        paiement.BonDeReceptions.Should().BeEmpty();
    }
}
