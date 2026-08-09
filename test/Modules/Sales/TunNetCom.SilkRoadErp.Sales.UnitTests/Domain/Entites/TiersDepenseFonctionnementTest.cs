using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class TiersDepenseFonctionnementTest
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var tiers = TiersDepenseFonctionnement.Create(
            nom: "SNT",
            tel: "123",
            adresse: "Tunis",
            matricule: "MAT-1",
            code: "C-1",
            codeCat: "CC-1",
            etbSec: "ES-1",
            mail: "snt@test.com",
            exonereRetenueSource: true);

        tiers.Nom.Should().Be("SNT");
        tiers.Tel.Should().Be("123");
        tiers.Adresse.Should().Be("Tunis");
        tiers.Matricule.Should().Be("MAT-1");
        tiers.Code.Should().Be("C-1");
        tiers.CodeCat.Should().Be("CC-1");
        tiers.EtbSec.Should().Be("ES-1");
        tiers.Mail.Should().Be("snt@test.com");
        tiers.ExonereRetenueSource.Should().BeTrue();
        tiers.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        tiers.FactureDepense.Should().BeEmpty();
        tiers.PaiementTiersDepense.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithDefaults_ShouldSetExonereRetenueSourceFalse()
    {
        var tiers = TiersDepenseFonctionnement.Create(
            nom: "SNT", tel: null, adresse: null, matricule: null, code: null, codeCat: null, etbSec: null, mail: null);

        tiers.ExonereRetenueSource.Should().BeFalse();
        tiers.Tel.Should().BeNull();
        tiers.Adresse.Should().BeNull();
        tiers.Matricule.Should().BeNull();
        tiers.Code.Should().BeNull();
        tiers.CodeCat.Should().BeNull();
        tiers.EtbSec.Should().BeNull();
        tiers.Mail.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateAllProperties()
    {
        var tiers = TiersDepenseFonctionnement.Create(
            nom: "Old", tel: null, adresse: null, matricule: null, code: null, codeCat: null, etbSec: null, mail: null);

        tiers.Update(
            nom: "New",
            tel: "456",
            adresse: "Sousse",
            matricule: "MAT-2",
            code: "C-2",
            codeCat: "CC-2",
            etbSec: "ES-2",
            mail: "new@test.com",
            exonereRetenueSource: true);

        tiers.Nom.Should().Be("New");
        tiers.Tel.Should().Be("456");
        tiers.Adresse.Should().Be("Sousse");
        tiers.Matricule.Should().Be("MAT-2");
        tiers.Code.Should().Be("C-2");
        tiers.CodeCat.Should().Be("CC-2");
        tiers.EtbSec.Should().Be("ES-2");
        tiers.Mail.Should().Be("new@test.com");
        tiers.ExonereRetenueSource.Should().BeTrue();
    }
}
