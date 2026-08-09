using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class BanqueTest
{
    [Fact]
    public void CreateBanque_ShouldSetProperties()
    {
        var banque = Banque.CreateBanque("BIAT");

        banque.Nom.Should().Be("BIAT");
        banque.Id.Should().Be(0);
        banque.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        banque.PaiementClient.Should().BeEmpty();
        banque.PaiementFournisseur.Should().BeEmpty();
        banque.CompteBancaire.Should().BeEmpty();
    }

    [Fact]
    public void UpdateBanque_ShouldUpdateNom()
    {
        var banque = Banque.CreateBanque("BIAT");

        banque.UpdateBanque("Amen Bank");

        banque.Nom.Should().Be("Amen Bank");
    }
}
