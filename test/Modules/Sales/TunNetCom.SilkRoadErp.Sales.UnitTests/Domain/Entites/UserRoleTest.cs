using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class UserRoleTest
{
    [Fact]
    public void CreateUserRole_ShouldSetProperties()
    {
        var userRole = UserRole.CreateUserRole(userId: 1, roleId: 2);

        userRole.UserId.Should().Be(1);
        userRole.RoleId.Should().Be(2);
        userRole.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        userRole.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
