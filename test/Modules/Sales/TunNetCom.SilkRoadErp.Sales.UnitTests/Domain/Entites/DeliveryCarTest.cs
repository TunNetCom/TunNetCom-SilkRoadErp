using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class DeliveryCarTest
{
    [Fact]
    public void NewDeliveryCar_ShouldHaveDefaults()
    {
        var car = new DeliveryCar();

        car.Matricule.Should().BeEmpty();
        car.Owner.Should().BeEmpty();
        car.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        car.BonDeLivraisons.Should().BeEmpty();
    }

    [Fact]
    public void Setters_ShouldAssignValues()
    {
        var car = new DeliveryCar
        {
            Id = 3,
            Matricule = "123 TUN 456",
            Owner = "Société X"
        };

        car.Id.Should().Be(3);
        car.Matricule.Should().Be("123 TUN 456");
        car.Owner.Should().Be("Société X");
    }
}
