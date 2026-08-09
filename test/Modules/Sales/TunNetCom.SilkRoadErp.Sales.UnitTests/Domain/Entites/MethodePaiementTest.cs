namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class MethodePaiementTest
{
    [Fact]
    public void Enum_ShouldHaveAllValues()
    {
        Enum.GetValues<MethodePaiement>().Should().Contain(new[]
        {
            MethodePaiement.Espece,
            MethodePaiement.Cheque,
            MethodePaiement.Traite,
            MethodePaiement.Virement,
            MethodePaiement.Tpe
        });
    }

    [Fact]
    public void Consts_ShouldMatchEnumNames()
    {
        MethodePaiementConsts.Espece.Should().Be(nameof(MethodePaiement.Espece));
        MethodePaiementConsts.Cheque.Should().Be(nameof(MethodePaiement.Cheque));
        MethodePaiementConsts.Traite.Should().Be(nameof(MethodePaiement.Traite));
        MethodePaiementConsts.Virement.Should().Be(nameof(MethodePaiement.Virement));
        MethodePaiementConsts.Tpe.Should().Be(nameof(MethodePaiement.Tpe));
    }
}
