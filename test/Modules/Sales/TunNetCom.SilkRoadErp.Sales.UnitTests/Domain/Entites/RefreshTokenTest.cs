using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Domain.Entites;

public class RefreshTokenTest
{
    [Fact]
    public void CreateRefreshToken_ShouldSetProperties()
    {
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var token = RefreshToken.CreateRefreshToken(
            userId: 1,
            token: "token-value",
            expiresAt: expiresAt);

        token.UserId.Should().Be(1);
        token.Token.Should().Be("token-value");
        token.ExpiresAt.Should().Be(expiresAt);
        token.IsRevoked.Should().BeFalse();
        token.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        token.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }

    [Fact]
    public void Revoke_ShouldSetIsRevokedAndRevokedAt()
    {
        var token = RefreshToken.CreateRefreshToken(1, "token-value", DateTime.UtcNow.AddDays(1));

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
        token.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void IsExpired_WhenExpired_ShouldReturnTrue()
    {
        var token = RefreshToken.CreateRefreshToken(1, "token-value", DateTime.UtcNow.AddDays(-1));

        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WhenNotExpired_ShouldReturnFalse()
    {
        var token = RefreshToken.CreateRefreshToken(1, "token-value", DateTime.UtcNow.AddDays(1));

        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNotRevokedAndNotExpired_ShouldReturnTrue()
    {
        var token = RefreshToken.CreateRefreshToken(1, "token-value", DateTime.UtcNow.AddDays(1));

        token.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenRevoked_ShouldReturnFalse()
    {
        var token = RefreshToken.CreateRefreshToken(1, "token-value", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        token.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenExpired_ShouldReturnFalse()
    {
        var token = RefreshToken.CreateRefreshToken(1, "token-value", DateTime.UtcNow.AddDays(-1));

        token.IsValid.Should().BeFalse();
    }
}
