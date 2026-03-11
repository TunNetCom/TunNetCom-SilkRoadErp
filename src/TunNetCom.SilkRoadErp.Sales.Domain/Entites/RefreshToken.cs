#nullable enable
using System;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class RefreshToken : ITenantEntity
{
    private RefreshToken()
    {
    }

    public static RefreshToken CreateRefreshToken(
        int userId,
        string token,
        DateTime expiresAt)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsValid => !IsRevoked && !IsExpired;

    public int Id { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int UserId { get; private set; }

    public string Token { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public virtual User User { get; set; } = null!;
}

