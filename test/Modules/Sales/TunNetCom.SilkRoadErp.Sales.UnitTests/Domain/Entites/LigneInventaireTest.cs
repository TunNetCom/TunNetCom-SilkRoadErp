using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class LigneInventaireTest
{
    [Fact]
    public void CreateLigneInventaire_ShouldSetProperties()
    {
        var ligne = LigneInventaire.CreateLigneInventaire(
            inventaireId: 3,
            refProduit: "PRD-001",
            quantiteTheorique: 10,
            quantiteReelle: 8,
            prixHt: 25.50m,
            dernierPrixAchat: 20m);

        ligne.InventaireId.Should().Be(3);
        ligne.RefProduit.Should().Be("PRD-001");
        ligne.QuantiteTheorique.Should().Be(10);
        ligne.QuantiteReelle.Should().Be(8);
        ligne.PrixHt.Should().Be(25.50m);
        ligne.DernierPrixAchat.Should().Be(20m);
        ligne.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void UpdateLigneInventaire_ShouldUpdateProperties()
    {
        var ligne = LigneInventaire.CreateLigneInventaire(1, "PRD-001", 10, 8, 25.5m, 20m);

        ligne.UpdateLigneInventaire(quantiteReelle: 12, prixHt: 30m, dernierPrixAchat: 22m);

        ligne.QuantiteReelle.Should().Be(12);
        ligne.PrixHt.Should().Be(30m);
        ligne.DernierPrixAchat.Should().Be(22m);
    }
}
