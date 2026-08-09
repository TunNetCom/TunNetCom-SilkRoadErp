using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RolePermissionTest
{
    [Fact]
    public void CreateRolePermission_ShouldSetProperties()
    {
        var rolePermission = RolePermission.CreateRolePermission(roleId: 1, permissionId: 2);

        rolePermission.RoleId.Should().Be(1);
        rolePermission.PermissionId.Should().Be(2);
        rolePermission.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        rolePermission.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
