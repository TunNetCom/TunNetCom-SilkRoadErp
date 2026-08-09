namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class PermissionTest
{
    [Fact]
    public void CreatePermission_ShouldSetProperties()
    {
        var permission = Permission.CreatePermission("read", "Can read");

        permission.Name.Should().Be("read");
        permission.Description.Should().Be("Can read");
        permission.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        permission.RolePermissions.Should().BeEmpty();
    }

    [Fact]
    public void CreatePermission_WhenDescriptionNull_ShouldKeepNull()
    {
        var permission = Permission.CreatePermission("read");

        permission.Description.Should().BeNull();
    }

    [Fact]
    public void UpdatePermission_ShouldUpdateDescriptionAndTimestamp()
    {
        var permission = Permission.CreatePermission("read", "Old");

        permission.UpdatePermission("New");

        permission.Description.Should().Be("New");
        permission.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdatePermission_WhenNullDescription_ShouldKeepExisting()
    {
        var permission = Permission.CreatePermission("read", "Old");

        permission.UpdatePermission(null);

        permission.Description.Should().Be("Old");
        permission.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetId_ShouldSetId()
    {
        var permission = Permission.CreatePermission("read");

        permission.SetId(9);

        permission.Id.Should().Be(9);
    }
}
