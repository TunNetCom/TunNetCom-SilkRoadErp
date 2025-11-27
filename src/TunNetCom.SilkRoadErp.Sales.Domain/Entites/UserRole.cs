#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class UserRole
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

    public int RoleId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public virtual User User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

