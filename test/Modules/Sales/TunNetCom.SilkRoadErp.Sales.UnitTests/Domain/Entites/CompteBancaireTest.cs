using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class CompteBancaireTest
{
    [Fact]
    public void CreateCompteBancaire_ShouldSetProperties()
    {
        var compte = CompteBancaire.CreateCompteBancaire(
            banqueId: 1,
            codeEtablissement: "011",
            codeAgence: "123",
            numeroCompte: "123456789",
            cleRib: "25",
            libelle: "Compte principal");

        compte.BanqueId.Should().Be(1);
        compte.CodeEtablissement.Should().Be("011");
        compte.CodeAgence.Should().Be("123");
        compte.NumeroCompte.Should().Be("123456789");
        compte.CleRib.Should().Be("25");
        compte.Libelle.Should().Be("Compte principal");
        compte.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void CreateCompteBancaire_WhenLibelleNull_ShouldKeepNull()
    {
        var compte = CompteBancaire.CreateCompteBancaire(
            banqueId: 1,
            codeEtablissement: "011",
            codeAgence: "123",
            numeroCompte: "123456789",
            cleRib: "25");

        compte.Libelle.Should().BeNull();
    }

    [Fact]
    public void UpdateCompteBancaire_ShouldUpdateAllProperties()
    {
        var compte = CompteBancaire.CreateCompteBancaire(
            banqueId: 1,
            codeEtablissement: "011",
            codeAgence: "123",
            numeroCompte: "123456789",
            cleRib: "25");

        compte.UpdateCompteBancaire(
            banqueId: 2,
            codeEtablissement: "022",
            codeAgence: "456",
            numeroCompte: "987654321",
            cleRib: "74",
            libelle: "Compte secondaire");

        compte.BanqueId.Should().Be(2);
        compte.CodeEtablissement.Should().Be("022");
        compte.CodeAgence.Should().Be("456");
        compte.NumeroCompte.Should().Be("987654321");
        compte.CleRib.Should().Be("74");
        compte.Libelle.Should().Be("Compte secondaire");
    }
}
