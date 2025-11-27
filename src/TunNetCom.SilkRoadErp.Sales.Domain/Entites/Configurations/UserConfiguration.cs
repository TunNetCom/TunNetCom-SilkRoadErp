using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

#nullable disable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_User");

        entity.ToTable("Users");

        entity.HasIndex(e => e.Username).IsUnique();
        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.Id)
            .HasColumnName("Id");

        entity.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Username");

        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("Email");

        entity.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("PasswordHash");

        entity.Property(e => e.FirstName)
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        entity.Property(e => e.LastName)
            .HasMaxLength(100)
            .HasColumnName("LastName");

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        entity.Property(e => e.UpdatedAt)
            .HasColumnName("UpdatedAt");

        entity.HasMany(e => e.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

