using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RoleTest
{
    [Fact]
    public void CreateRole_ShouldSetProperties()
    {
        var role = Role.CreateRole("Admin", "Administrator");

        role.Name.Should().Be("Admin");
        role.Description.Should().Be("Administrator");
        role.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        role.TenantId.Should().Be(TenantConstants.DefaultTenantId);
        role.UserRoles.Should().BeEmpty();
        role.RolePermissions.Should().BeEmpty();
    }

    [Fact]
    public void CreateRole_WhenDescriptionNull_ShouldKeepNull()
    {
        var role = Role.CreateRole("User");

        role.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateRole_ShouldUpdateDescriptionAndTimestamp()
    {
        var role = Role.CreateRole("Admin", "Old");

        role.UpdateRole("New description");

        role.Description.Should().Be("New description");
        role.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateRole_WhenNullDescription_ShouldKeepExisting()
    {
        var role = Role.CreateRole("Admin", "Old");

        role.UpdateRole(null);

        role.Description.Should().Be("Old");
        role.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetId_ShouldSetId()
    {
        var role = Role.CreateRole("Admin");

        role.SetId(5);

        role.Id.Should().Be(5);
    }
}
