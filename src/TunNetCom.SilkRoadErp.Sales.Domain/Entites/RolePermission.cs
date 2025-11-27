#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class RolePermission
{
    private RolePermission()
    {
    }

    public static RolePermission CreateRolePermission(int roleId, int permissionId)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public int RoleId { get; private set; }

    public int PermissionId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}

