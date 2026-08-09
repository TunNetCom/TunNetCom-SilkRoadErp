using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class LigneRetourMarchandiseFournisseurTest
{
    [Fact]
    public void CreateRetourLine_ShouldComputeTotals()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: 1,
            productRef: "PRD-001",
            designationLigne: "Produit",
            quantity: 10,
            unitPrice: 100m,
            discount: 10,
            tax: 19);

        ligne.RefProduit.Should().Be("PRD-001");
        ligne.DesignationLi.Should().Be("Produit");
        ligne.QteLi.Should().Be(10);
        ligne.PrixHt.Should().Be(100m);
        ligne.Remise.Should().Be(10);
        ligne.Tva.Should().Be(19);
        ligne.TotHt.Should().Be(900m);
        ligne.TotTtc.Should().Be(1071m);
        ligne.QteRecue.Should().Be(0);
        ligne.RetourMarchandiseFournisseurId.Should().Be(1);
        ligne.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void CreateRetourLine_WithoutDiscountAndTax_ShouldComputeTotals()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: 1,
            productRef: "PRD-001",
            designationLigne: "Produit",
            quantity: 5,
            unitPrice: 100m,
            discount: 0,
            tax: 0);

        ligne.TotHt.Should().Be(500m);
        ligne.TotTtc.Should().Be(500m);
        ligne.QteRecue.Should().Be(0);
    }

    [Fact]
    public void CreateRetourLine_WithQteRecue_ShouldSetIt()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            retourMarchandiseFournisseurId: 1,
            productRef: "PRD-001",
            designationLigne: "Produit",
            quantity: 10,
            unitPrice: 100m,
            discount: 0,
            tax: 19,
            qteRecue: 4);

        ligne.QteRecue.Should().Be(4);
    }

    [Fact]
    public void EnregistrerReception_ShouldSetReceivedQuantity()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19);

        ligne.EnregistrerReception(quantiteRecue: 6, utilisateur: "admin");

        ligne.QteRecue.Should().Be(6);
        ligne.DateReception.Should().NotBeNull();
        ligne.UtilisateurReception.Should().Be("admin");
    }

    [Fact]
    public void EnregistrerReception_WhenNegativeQuantity_ShouldThrow()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19);

        var act = () => ligne.EnregistrerReception(quantiteRecue: -1, utilisateur: "admin");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*ne peut pas être négative*");
    }

    [Fact]
    public void EnregistrerReception_WhenQuantityExceedsReturned_ShouldThrow()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19);

        var act = () => ligne.EnregistrerReception(quantiteRecue: 11, utilisateur: "admin");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*ne peut pas dépasser la quantité retournée*");
    }

    [Fact]
    public void GetQuantiteEnAttente_ShouldReturnRemaining()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19, qteRecue: 4);

        ligne.GetQuantiteEnAttente().Should().Be(6);
    }

    [Fact]
    public void GetQuantiteEnAttente_WhenFullyReceived_ShouldReturnZero()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19, qteRecue: 10);

        ligne.GetQuantiteEnAttente().Should().Be(0);
    }

    [Fact]
    public void EstEntierementRecue_WhenFullyReceived_ShouldReturnTrue()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19, qteRecue: 10);

        ligne.EstEntierementRecue.Should().BeTrue();
    }

    [Fact]
    public void EstEntierementRecue_WhenPartiallyReceived_ShouldReturnFalse()
    {
        var ligne = LigneRetourMarchandiseFournisseur.CreateRetourLine(
            1, "PRD-001", "Produit", 10, 100m, 0, 19, qteRecue: 4);

        ligne.EstEntierementRecue.Should().BeFalse();
    }
}
