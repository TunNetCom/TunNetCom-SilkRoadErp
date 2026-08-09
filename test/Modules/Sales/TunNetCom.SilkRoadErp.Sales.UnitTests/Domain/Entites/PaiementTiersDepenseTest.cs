using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PaiementTiersDepenseTest
{
    private static PaiementTiersDepense CreatePaiement()
    {
        return PaiementTiersDepense.Create(
            numeroTransactionBancaire: "TRX-001",
            tiersDepenseFonctionnementId: 1,
            accountingYearId: 2024,
            montant: 300m,
            datePaiement: new DateTime(2024, 7, 15),
            methodePaiement: MethodePaiement.Virement,
            factureDepenseIds: new[] { 1, 2 },
            numeroChequeTraite: null,
            banqueId: 3,
            dateEcheance: new DateTime(2024, 8, 15),
            commentaire: "Loyer",
            ribCodeEtab: "011",
            ribCodeAgence: "123",
            ribNumeroCompte: "456789",
            ribCle: "25",
            documentStoragePath: "/docs/paiement.pdf",
            mois: 7);
    }

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var paiement = CreatePaiement();

        paiement.NumeroTransactionBancaire.Should().Be("TRX-001");
        paiement.TiersDepenseFonctionnementId.Should().Be(1);
        paiement.AccountingYearId.Should().Be(2024);
        paiement.Montant.Should().Be(300m);
        paiement.DatePaiement.Should().Be(new DateTime(2024, 7, 15));
        paiement.MethodePaiement.Should().Be(MethodePaiement.Virement);
        paiement.BanqueId.Should().Be(3);
        paiement.DateEcheance.Should().Be(new DateTime(2024, 8, 15));
        paiement.Commentaire.Should().Be("Loyer");
        paiement.RibCodeEtab.Should().Be("011");
        paiement.RibCodeAgence.Should().Be("123");
        paiement.RibNumeroCompte.Should().Be("456789");
        paiement.RibCle.Should().Be("25");
        paiement.DocumentStoragePath.Should().Be("/docs/paiement.pdf");
        paiement.Mois.Should().Be(7);
        paiement.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        paiement.FactureDepenses.Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldUpdatePropertiesAndCollection()
    {
        var paiement = CreatePaiement();

        paiement.Update(
            numeroTransactionBancaire: "TRX-002",
            tiersDepenseFonctionnementId: 2,
            accountingYearId: 2025,
            montant: 400m,
            datePaiement: new DateTime(2025, 1, 10),
            methodePaiement: MethodePaiement.Espece,
            factureDepenseIds: new[] { 5 },
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
        paiement.TiersDepenseFonctionnementId.Should().Be(2);
        paiement.AccountingYearId.Should().Be(2025);
        paiement.Montant.Should().Be(400m);
        paiement.MethodePaiement.Should().Be(MethodePaiement.Espece);
        paiement.Commentaire.Should().Be("Updated");
        paiement.Mois.Should().Be(1);
        paiement.DateModification.Should().NotBeNull();
        paiement.FactureDepenses.Should().HaveCount(1);
        paiement.FactureDepenses.Should().ContainSingle().Which.FactureDepenseId.Should().Be(5);
    }

    [Fact]
    public void Update_WhenFactureIdsNull_ShouldKeepExistingCollection()
    {
        var paiement = CreatePaiement();

        paiement.Update(
            numeroTransactionBancaire: "TRX-002",
            tiersDepenseFonctionnementId: 1,
            accountingYearId: 2024,
            montant: 300m,
            datePaiement: new DateTime(2024, 7, 15),
            methodePaiement: MethodePaiement.Virement,
            factureDepenseIds: null,
            numeroChequeTraite: null,
            banqueId: 3,
            dateEcheance: new DateTime(2024, 8, 15),
            commentaire: "Loyer",
            ribCodeEtab: "011",
            ribCodeAgence: "123",
            ribNumeroCompte: "456789",
            ribCle: "25",
            documentStoragePath: "/docs/paiement.pdf",
            mois: 7);

        paiement.FactureDepenses.Should().BeEmpty();
    }
}
