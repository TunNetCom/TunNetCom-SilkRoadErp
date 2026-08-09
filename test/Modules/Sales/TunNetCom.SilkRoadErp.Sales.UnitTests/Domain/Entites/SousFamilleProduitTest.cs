using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class SousFamilleProduitTest
{
    [Fact]
    public void CreateSousFamilleProduit_ShouldSetProperties()
    {
        var sousFamille = SousFamilleProduit.CreateSousFamilleProduit(
            nom: "Ordinateurs",
            familleProduitId: 2);

        sousFamille.Nom.Should().Be("Ordinateurs");
        sousFamille.FamilleProduitId.Should().Be(2);
        sousFamille.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        sousFamille.Produits.Should().BeEmpty();
    }

    [Fact]
    public void UpdateSousFamilleProduit_ShouldUpdateProperties()
    {
        var sousFamille = SousFamilleProduit.CreateSousFamilleProduit("Ordinateurs", 2);

        sousFamille.UpdateSousFamilleProduit(nom: "Imprimantes", familleProduitId: 3);

        sousFamille.Nom.Should().Be("Imprimantes");
        sousFamille.FamilleProduitId.Should().Be(3);
    }

    [Fact]
    public void SetId_ShouldUpdateId()
    {
        var sousFamille = SousFamilleProduit.CreateSousFamilleProduit("Ordinateurs", 2);

        sousFamille.SetId(7);

        sousFamille.Id.Should().Be(7);
    }
}
