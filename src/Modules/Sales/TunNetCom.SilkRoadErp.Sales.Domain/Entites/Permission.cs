#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class Permission
{
    private Permission()
    {
        RolePermissions = new List<RolePermission>();
    }

    public static Permission CreatePermission(
        string name,
        string? description = null)
    {
        return new Permission
        {
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdatePermission(string? description = null)
    {
        if (description != null)
            Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetId(int id)
    {
        Id = id;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; }
}

