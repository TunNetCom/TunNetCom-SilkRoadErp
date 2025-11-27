#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class User
{
    private User()
    {
        UserRoles = new List<UserRole>();
        RefreshTokens = new List<RefreshToken>();
    }

    public static User CreateUser(
        string username,
        string email,
        string passwordHash,
        string? firstName = null,
        string? lastName = null,
        bool isActive = true)
    {
        return new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateUser(
        string? email = null,
        string? firstName = null,
        string? lastName = null,
        bool? isActive = null)
    {
        if (email != null)
            Email = email;
        if (firstName != null)
            FirstName = firstName;
        if (lastName != null)
            LastName = lastName;
        if (isActive.HasValue)
            IsActive = isActive.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetId(int id)
    {
        Id = id;
    }

    public int Id { get; private set; }

    public string Username { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}

