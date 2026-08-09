using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class EcheanceDesFournisseursTest
{
    [Fact]
    public void Setters_ShouldAssignValues()
    {
        var date = new DateTime(2024, 6, 30);

        var echeance = new EcheanceDesFournisseurs
        {
            Id = 1,
            DateEcheance = date,
            NumCheque = 999,
            Montant = 1500.75m,
            FournisseurId = 42
        };

        echeance.Id.Should().Be(1);
        echeance.DateEcheance.Should().Be(date);
        echeance.NumCheque.Should().Be(999);
        echeance.Montant.Should().Be(1500.75m);
        echeance.FournisseurId.Should().Be(42);
        echeance.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
