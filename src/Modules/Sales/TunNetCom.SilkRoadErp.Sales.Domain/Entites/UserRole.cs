#nullable enable
using System;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class UserRole : ITenantEntity
{
    private UserRole()
    {
    }

    public static UserRole CreateUserRole(int userId, int roleId)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public int UserId { get; private set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int RoleId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public virtual User User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

