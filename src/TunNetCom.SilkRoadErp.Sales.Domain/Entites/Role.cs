#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class Role
{
    private Role()
    {
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
    }

    public static Role CreateRole(
        string name,
        string? description = null)
    {
        return new Role
        {
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateRole(string? description = null)
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

    public virtual ICollection<UserRole> UserRoles { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; }
}

