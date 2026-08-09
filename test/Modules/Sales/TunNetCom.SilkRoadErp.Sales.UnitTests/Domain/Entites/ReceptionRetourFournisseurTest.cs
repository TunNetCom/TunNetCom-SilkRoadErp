using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class ReceptionRetourFournisseurTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var date = new DateTime(2024, 7, 10);

        var reception = ReceptionRetourFournisseur.Create(
            retourMarchandiseFournisseurId: 1,
            dateReception: date,
            utilisateur: "admin",
            commentaire: "Reparé");

        reception.RetourMarchandiseFournisseurId.Should().Be(1);
        reception.DateReception.Should().Be(date);
        reception.Utilisateur.Should().Be("admin");
        reception.Commentaire.Should().Be("Reparé");
        reception.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void Create_WhenCommentaireNull_ShouldKeepNull()
    {
        var reception = ReceptionRetourFournisseur.Create(1, DateTime.Now, "admin");

        reception.Commentaire.Should().BeNull();
    }
}
