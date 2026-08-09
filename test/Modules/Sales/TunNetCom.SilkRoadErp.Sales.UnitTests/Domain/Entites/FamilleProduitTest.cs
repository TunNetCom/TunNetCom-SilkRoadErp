using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class FamilleProduitTest
{
    [Fact]
    public void CreateFamilleProduit_ShouldSetProperties()
    {
        var famille = FamilleProduit.CreateFamilleProduit("Electronique");

        famille.Nom.Should().Be("Electronique");
        famille.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        famille.SousFamilles.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFamilleProduit_ShouldUpdateNom()
    {
        var famille = FamilleProduit.CreateFamilleProduit("Electronique");

        famille.UpdateFamilleProduit("Informatique");

        famille.Nom.Should().Be("Informatique");
    }

    [Fact]
    public void SetId_ShouldUpdateId()
    {
        var famille = FamilleProduit.CreateFamilleProduit("Electronique");

        famille.SetId(10);

        famille.Id.Should().Be(10);
    }
}
