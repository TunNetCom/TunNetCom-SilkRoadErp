using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class UserTest
{
    [Fact]
    public void CreateUser_ShouldSetAllProperties()
    {
        var user = User.CreateUser(
            username: "admin",
            email: "admin@test.com",
            passwordHash: "hash",
            firstName: "John",
            lastName: "Doe");

        user.Username.Should().Be("admin");
        user.Email.Should().Be("admin@test.com");
        user.PasswordHash.Should().Be("hash");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void CreateUser_WithInactive_ShouldSetIsActiveFalse()
    {
        var user = User.CreateUser(
            username: "admin",
            email: "admin@test.com",
            passwordHash: "hash",
            isActive: false);

        user.IsActive.Should().BeFalse();
        user.FirstName.Should().BeNull();
        user.LastName.Should().BeNull();
    }

    [Fact]
    public void UpdateUser_ShouldUpdateProvidedValues()
    {
        var user = User.CreateUser("admin", "old@test.com", "hash");

        user.UpdateUser(
            email: "new@test.com",
            firstName: "Jane",
            lastName: "Smith",
            isActive: false);

        user.Email.Should().Be("new@test.com");
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateUser_WhenNullValues_ShouldKeepExistingValues()
    {
        var user = User.CreateUser("admin", "old@test.com", "hash", firstName: "John");

        user.UpdateUser();

        user.Email.Should().Be("old@test.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().BeNull();
        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ChangePassword_ShouldUpdatePasswordHashAndTimestamp()
    {
        var user = User.CreateUser("admin", "admin@test.com", "old-hash");

        user.ChangePassword("new-hash");

        user.PasswordHash.Should().Be("new-hash");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetId_ShouldSetId()
    {
        var user = User.CreateUser("admin", "admin@test.com", "hash");

        user.SetId(42);

        user.Id.Should().Be(42);
    }

    [Fact]
    public void NewUser_ShouldInitializeCollections()
    {
        var user = User.CreateUser("admin", "admin@test.com", "hash");

        user.UserRoles.Should().BeEmpty();
        user.RefreshTokens.Should().BeEmpty();
    }
}
